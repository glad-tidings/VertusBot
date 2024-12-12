Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers

Public Class VertusApi
    Private ReadOnly client As HttpClient

    Public Sub New(queryID As String, queryIndex As Integer, Proxy As Proxy())
        Dim FProxy = Proxy.Where(Function(x) x.Index = queryIndex)
        If FProxy.Count <> 0 Then
            If FProxy(0).Proxy <> "" Then
                Dim handler = New HttpClientHandler With {.Proxy = New WebProxy With {.Address = New Uri(FProxy(0).Proxy)}}
                client = New HttpClient(handler) With {.Timeout = New TimeSpan(0, 0, 30)}
            Else
                client = New HttpClient With {.Timeout = New TimeSpan(0, 0, 30)}
            End If
        Else
            client = New HttpClient With {.Timeout = New TimeSpan(0, 0, 30)}
        End If
        client.DefaultRequestHeaders.CacheControl = New CacheControlHeaderValue With {.NoCache = True, .NoStore = True, .MaxAge = TimeSpan.FromSeconds(0)}
        client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9")
        client.DefaultRequestHeaders.Add("Connection", "keep-alive")
        client.DefaultRequestHeaders.Add("Origin", "https://thevertus.app")
        client.DefaultRequestHeaders.Add("Priority", "u=1, i")
        client.DefaultRequestHeaders.Add("Referer", "https://thevertus.app/")
        client.DefaultRequestHeaders.Add("sec-ch-ua", """Chromium"";v=""130"", ""Microsoft Edge"";v=""130"", ""Not?A_Brand"";v=""99"", ""Microsoft Edge WebView2"";v=""130""")
        client.DefaultRequestHeaders.Add("Sec-Fetch-Dest", "empty")
        client.DefaultRequestHeaders.Add("Sec-Fetch-Mode", "cors")
        client.DefaultRequestHeaders.Add("Sec-Fetch-Site", "same-site")
        client.DefaultRequestHeaders.Add("User-Agent", Tools.getUserAgents(queryIndex))
        client.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*")
        client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", "?0")
        client.DefaultRequestHeaders.Add("sec-ch-ua-platform", $"""{Tools.getUserAgents(queryIndex, True)}""")
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {queryID}")
    End Sub

    Public Async Function VAPIGet(requestUri As String) As Task(Of HttpResponseMessage)
        Try
            Return Await client.GetAsync(requestUri)
        Catch ex As Exception
            Return New HttpResponseMessage With {.StatusCode = HttpStatusCode.ExpectationFailed, .ReasonPhrase = ex.Message}
        End Try
    End Function

    Public Async Function VAPIPost(requestUri As String, content As HttpContent) As Task(Of HttpResponseMessage)
        Try
            Return Await client.PostAsync(requestUri, content)
        Catch ex As Exception
            Return New HttpResponseMessage With {.StatusCode = HttpStatusCode.ExpectationFailed, .ReasonPhrase = ex.Message}
        End Try
    End Function
End Class
