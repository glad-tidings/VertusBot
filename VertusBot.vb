Imports System.Net
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Text
Imports System.Text.Json

Public Class VertusBot

    Private ReadOnly PubQuery As VertusQuery
    Private ReadOnly PubProxy As Proxy()
    Public ReadOnly UserDetail As VertusUserDataResponse
    Public ReadOnly HasError As Boolean
    Public ReadOnly ErrorMessage As String
    Public ReadOnly IPAddress As String

    Public Sub New(Query As VertusQuery, Proxy As Proxy())
        PubQuery = Query
        PubProxy = Proxy
        IPAddress = GetIP().Result
        PubQuery.Auth = getSession().Result
        Dim GetUserDetail = VertusUserData().Result
        Dim Check = VertusCheck().Result
        Dim Ping = VertusPing().Result
        If GetUserDetail IsNot Nothing And Check And Ping Then
            UserDetail = GetUserDetail
            HasError = False
            ErrorMessage = ""
        Else
            HasError = True
            ErrorMessage = "get profile data failed"
        End If
    End Sub

    Private Async Function GetIP() As Task(Of String)
        Dim client As HttpClient
        Dim FProxy = PubProxy.Where(Function(x) x.Index = PubQuery.Index)
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
        Dim httpResponse As HttpResponseMessage = Nothing
        Try
            httpResponse = Await client.GetAsync($"https://httpbin.org/ip")
        Catch ex As Exception
        End Try
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of httpbin)(responseStream)
                Return responseJson.Origin
            Else
                Return ""
            End If
        Else
            Return ""
        End If
    End Function

    Private Async Function getSession() As Task(Of String)
        Dim vw As TelegramMiniApp.WebView = New TelegramMiniApp.WebView(PubQuery.API_ID, PubQuery.API_HASH, PubQuery.Name, PubQuery.Phone, "Vertus_App_bot", "https://thevertus.app/")
        Dim url As String = Await vw.Get_URL()
        If url <> "" Then
            Return url.Split(New String() {"tgWebAppData="}, StringSplitOptions.None)(1).Split(New String() {"&tgWebAppVersion"}, StringSplitOptions.None)(0)
        Else
            Return ""
        End If
    End Function

    Private Async Function VertusUserData() As Task(Of VertusUserDataResponse)
        Dim VAPI As New VertusApi(PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim httpResponse = Await VAPI.VAPIPost("https://api.thevertus.app/users/get-data", Nothing)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of VertusUserDataResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Private Async Function VertusCheck() As Task(Of Boolean)
        Dim VAPI As New VertusApi(PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim httpResponse = Await VAPI.VAPIGet("https://api.thevertus.app/queue/check")
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of VertusCheckResponse)(responseStream)
                Return responseJson.IsSuccess
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Private Async Function VertusPing() As Task(Of Boolean)
        Dim VAPI As New VertusApi(PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim httpResponse = Await VAPI.VAPIGet("https://api.thevertus.app/ping")
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of VertusPingResponse)(responseStream)
                Return (responseJson.Message = "Pong!")
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Public Async Function VertusCards() As Task(Of List(Of VertusCards))
        Dim VC As New List(Of VertusCards)
        Dim VAPI As New VertusApi(PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim httpResponse = Await VAPI.VAPIGet("https://api.thevertus.app/upgrade-cards")
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of VertusCardsResponse)(responseStream)
                For Each item In responseJson.EconomyCards
                    Dim EVC As New VertusCards With {
                            .CardName = item.CardName,
                            .NextValue = item.NextValue,
                            .Id = item.Id,
                            .IsLocked = item.IsLocked,
                            .IsTon = item.IsTon,
                            .IsUpgradable = item.IsUpgradable,
                            .Level = item.CurrentLevel,
                            .Type = item.Type,
                            .Value = item.CurrentValue
                        }

                    If item.CurrentLevel < item.Levels.Count Then
                        EVC.Cost = item.Levels(item.CurrentLevel).Cost
                    Else
                        EVC.Cost = 0
                    End If

                    VC.Add(EVC)
                Next

                For Each item In responseJson.MilitaryCards
                    Dim EVC As New VertusCards With {
                            .CardName = item.CardName,
                            .NextValue = item.NextValue,
                            .Id = item.Id,
                            .IsLocked = item.IsLocked,
                            .IsTon = item.IsTon,
                            .IsUpgradable = item.IsUpgradable,
                            .Level = item.CurrentLevel,
                            .Type = item.Type,
                            .Value = item.CurrentValue
                        }

                    If item.CurrentLevel < item.Levels.Count Then
                        EVC.Cost = item.Levels(item.CurrentLevel).Cost
                    Else
                        EVC.Cost = 0
                    End If

                    VC.Add(EVC)
                Next

                For Each item In responseJson.ScienceCards
                    Dim EVC As New VertusCards With {
                            .CardName = item.CardName,
                            .NextValue = item.NextValue,
                            .Id = item.Id,
                            .IsLocked = item.IsLocked,
                            .IsTon = item.IsTon,
                            .IsUpgradable = item.IsUpgradable,
                            .Level = item.CurrentLevel,
                            .Type = item.Type,
                            .Value = item.CurrentValue
                        }

                    If item.CurrentLevel < item.Levels.Count Then
                        EVC.Cost = item.Levels(item.CurrentLevel).Cost
                    Else
                        EVC.Cost = 0
                    End If

                    VC.Add(EVC)
                Next

                Return VC
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function VertusUpgradeCard(cardId As String) As Task(Of VertusUpgradeCardResponse)
        Dim VAPI As New VertusApi(PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim request As New VertusUpgradeCardRequest With {.CardId = cardId}
        Dim serializedRequest = JsonSerializer.Serialize(request)
        Dim serializedRequestContent = New StringContent(serializedRequest, Encoding.UTF8, "application/json")
        Dim httpResponse = Await VAPI.VAPIPost("https://api.thevertus.app/upgrade-cards/upgrade", serializedRequestContent)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of VertusUpgradeCardResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function VertusMissions() As Task(Of VertusMissionsResponse)
        Dim VAPI As New VertusApi(PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim request As New VertusMissionsRequest With {.IsPremium = False, .LanguageCode = "en"}
        Dim serializedRequest = JsonSerializer.Serialize(request)
        Dim serializedRequestContent = New StringContent(serializedRequest, Encoding.UTF8, "application/json")
        Dim httpResponse = Await VAPI.VAPIPost("https://api.thevertus.app/missions/get", serializedRequestContent)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of VertusMissionsResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function VertusMissionsComplete(missionId As String) As Task(Of VertusMissionsCompleteResponse)
        Dim VAPI As New VertusApi(PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim request As New VertusMissionsCompleteRequest With {.MissionId = missionId}
        Dim serializedRequest = JsonSerializer.Serialize(request)
        Dim serializedRequestContent = New StringContent(serializedRequest, Encoding.UTF8, "application/json")
        Dim httpResponse = Await VAPI.VAPIPost("https://api.thevertus.app/upgrade-cards/upgrade", serializedRequestContent)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of VertusMissionsCompleteResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function VertusCollect() As Task(Of VertusCollectResponse)
        Dim VAPI As New VertusApi(PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim httpResponse = Await VAPI.VAPIPost("https://api.thevertus.app/game-service/collect", Nothing)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of VertusCollectResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function VertusCheckAdsgram() As Task(Of Boolean)
        Dim VAPI As New VertusApi(PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim httpResponse = Await VAPI.VAPIPost("https://api.thevertus.app/missions/check-adsgram", Nothing)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of VertusCheckResponse)(responseStream)
                Return responseJson.IsSuccess
            Else
                Return False
            End If
        Else
            Return False
        End If
    End Function

    Public Async Function VertusCompleteAdsgram() As Task(Of VertusAdsgramCompleteResponse)
        Dim VAPI As New VertusApi(PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim httpResponse = Await VAPI.VAPIPost("https://api.thevertus.app/missions/complete-adsgram", Nothing)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of VertusAdsgramCompleteResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function VertusDailyReward() As Task(Of VertusDailyRewardResponse)
        Dim VAPI As New VertusApi(PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim httpResponse = Await VAPI.VAPIPost("https://api.thevertus.app/users/claim-daily", Nothing)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of VertusDailyRewardResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function VertusCountries(groupId As String) As Task(Of VertusCountriesResponse)
        Dim VAPI As New VertusApi(PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim httpResponse = Await VAPI.VAPIGet($"https://api.thevertus.app/countries/{groupId}")
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of VertusCountriesResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function VertusUpgrade(upgrade As String) As Task(Of VertusUpgradeResponse)
        Dim VAPI As New VertusApi(PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim request As New VertusUpgradeRequest With {.Upgrade = upgrade}
        Dim serializedRequest = JsonSerializer.Serialize(request)
        Dim serializedRequestContent = New StringContent(serializedRequest, Encoding.UTF8, "application/json")
        Dim httpResponse = Await VAPI.VAPIPost("https://api.thevertus.app/users/upgrade", serializedRequestContent)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of VertusUpgradeResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function VertusAnswer() As Task(Of VertusAnswerResponse)
        Dim client As New HttpClient With {.Timeout = New TimeSpan(0, 0, 30)}
        client.DefaultRequestHeaders.CacheControl = New CacheControlHeaderValue With {.NoCache = True, .NoStore = True, .MaxAge = TimeSpan.FromSeconds(0)}
        Dim httpResponse As HttpResponseMessage = Nothing
        Try
            httpResponse = Await client.GetAsync("https://raw.githubusercontent.com/glad-tidings/VertusBot/refs/heads/main/codes.json")
        Catch ex As Exception
        End Try
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of VertusAnswerResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

    Public Async Function VertusCodes(code As String) As Task(Of VertusCodesResponse)
        Dim VAPI As New VertusApi(PubQuery.Auth, PubQuery.Index, PubProxy)
        Dim request As New VertusCodesRequest With {.Code = code}
        Dim serializedRequest = JsonSerializer.Serialize(request)
        Dim serializedRequestContent = New StringContent(serializedRequest, Encoding.UTF8, "application/json")
        Dim httpResponse = Await VAPI.VAPIPost("https://api.thevertus.app/codes/validate", serializedRequestContent)
        If httpResponse IsNot Nothing Then
            If httpResponse.IsSuccessStatusCode Then
                Dim responseStream = Await httpResponse.Content.ReadAsStreamAsync()
                Dim responseJson = Await JsonSerializer.DeserializeAsync(Of VertusCodesResponse)(responseStream)
                Return responseJson
            Else
                Return Nothing
            End If
        Else
            Return Nothing
        End If
    End Function

End Class
