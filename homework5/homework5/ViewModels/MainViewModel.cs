using System.Net.Http;
using System.Text.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Text.RegularExpressions;
using homework5.Models;
using ReactiveUI;
using HtmlAgilityPack;

namespace homework5.ViewModels;

public class MainViewModel : ViewModelBase
{
    private string _input = "", _searchReqResult = "";
    public string Input  {
        get => _input;
        set => this.RaiseAndSetIfChanged(ref _input, value);
    }

    public string SearchReqResult
    {
        get => _searchReqResult;
        set => this.RaiseAndSetIfChanged(ref _searchReqResult, value);
    }

    public ObservableCollection<TreeNode> ResultTree { get; } = new();
    public ReactiveCommand<Unit, Unit> SearchCommand { get; }

    public MainViewModel()
    {
        var search = async () =>
        {
            ResultTree.Clear();
            HttpClient bingReq = new();
            string wd = Uri.EscapeDataString(Input);
            string userAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:109.0) Gecko/20100101 Firefox/119.0";
            bingReq.DefaultRequestHeaders.Add("User-Agent", userAgent);
            var key = Environment.GetEnvironmentVariable("BING_SEARCH_V7_SUBSCRIPTION_KEY");
            if (key is null || key == "")
            {
                SearchReqResult = "请设置 BING_SEARCH_V7_SUBSCRIPTION_KEY 环境变量为您的 bing key\n";
                return;
            }
            bingReq.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key",
                Environment.GetEnvironmentVariable(key));

            var bingRes =
                await bingReq.GetAsync($"https://api.bing.microsoft.com/v7.0/search?q={wd}&count=10&mkt=zh-CN&responseFilter=Webpages");

            if (!bingRes.IsSuccessStatusCode)
            {
                SearchReqResult = "必应搜索失败" + $"\n{bingRes}";
            }
            else
            {
                // await bingRes.Content.LoadIntoBufferAsync(200);
                var json = await bingRes.Content.ReadAsStringAsync();
                using JsonDocument document = JsonDocument.Parse(json);
                JsonElement rootElement = document.RootElement;
                if (rootElement.GetProperty("_type").ToString() != "SearchResponse")
                {
                    SearchReqResult = "必应搜索失败" + $"\n{rootElement.GetProperty("errors")}";
                    return;
                }

                var webpages = rootElement.GetProperty("webPages").GetProperty("value").EnumerateArray();
                foreach (var nowPage in webpages.ToList())
                {
                    var showTitle = $"{nowPage.GetProperty("name")}:{nowPage.GetProperty("displayUrl")}";
                    TreeNode nowNode = new(showTitle);
                    HttpClient subPageReq = new();
                    subPageReq.DefaultRequestHeaders.Add("User-Agent", userAgent);
                    var subPageRep = await subPageReq.GetAsync(nowPage.GetProperty("url").ToString());
                    if (!subPageRep.IsSuccessStatusCode) continue;
                    HtmlDocument subPageDoc = new();
                    subPageDoc.Load(await subPageRep.Content.ReadAsStreamAsync());
                    var subPageDocNode = subPageDoc.DocumentNode;

                    var results = Regex.Matches(subPageDocNode.InnerText, """(1[3-9]\d[- ]?\d{4}[- ]?\d{4})|((\(?\d{3,4}\)-?)?\d{7,8})""")
                        .ToList();
                    foreach (var result in results.ToList())
                    {
                        nowNode.AddChild(new TreeNode(result.Value));
                    }

                    ResultTree.Add(nowNode);
                }

                SearchReqResult = string.Empty;
            }

        };
        SearchCommand = ReactiveCommand.CreateFromTask(search);
    }

}
