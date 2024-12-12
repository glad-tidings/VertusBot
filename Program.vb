Imports System.IO
Imports System.Text.Json
Imports System.Threading

Module Program
    Private proxies As Proxy()

    Sub Main()
        Console.WriteLine(" __     __        _             ____   ___ _____ 
 \ \   / /__ _ __| |_ _   _ ___| __ ) / _ \_   _|
  \ \ / / _ \ '__| __| | | / __|  _ \| | | || |  
   \ V /  __/ |  | |_| |_| \__ \ |_) | |_| || |  
    \_/ \___|_|   \__|\__,_|___/____/ \___/ |_|  
                                                 ")
        Console.WriteLine()
        Console.WriteLine("Github: https://github.com/glad-tidings/VertusBot")
        Console.WriteLine()
mainMenu:
        Console.Write("Select an option:
1. Run bot
2. Create session
>> ")
        Dim opt As String = Console.ReadLine()

        Dim queries As VertusQuery()
        Dim jsonConfig As String = ""
        Dim jsonProxy As String = ""
        Try
            jsonConfig = File.ReadAllText("Config.json")
        Catch ex As Exception
            Console.WriteLine("file 'Config.json' not found")
            GoTo Get_Error
        End Try
        Try
            jsonProxy = File.ReadAllText("Proxy.json")
        Catch ex As Exception
            Console.WriteLine("file 'Proxy.json' not found")
            GoTo Get_Error
        End Try
        Try
            queries = JsonSerializer.Deserialize(Of VertusQuery())(jsonConfig)
            proxies = JsonSerializer.Deserialize(Of Proxy())(jsonConfig)
        Catch ex As Exception
            Console.WriteLine("configuration is wrong")
            GoTo Get_Error
        End Try

        If Not String.IsNullOrEmpty(opt) Then
            Select Case opt
                Case "1"
                    Dim Cats As New Thread(
                        Sub()
                            For Each Query In queries.Where(Function(x) x.Active)
                                Dim BotThread As New Thread(Sub() VertusThread(Query))
                                BotThread.Start()

                                Thread.Sleep(120000)
                            Next
                        End Sub)
                    Cats.Start()
                Case "2"
                    For Each Query In queries
                        If Not File.Exists($"sessions\{Query.Name}.session") Then
                            Console.WriteLine()
                            Console.WriteLine($"Create session for account {Query.Name} ({Query.Phone})")
                            Dim vw As TelegramMiniApp.WebView = New TelegramMiniApp.WebView(Query.API_ID, Query.API_HASH, Query.Name, Query.Phone, "", "")
                            If vw.Save_Session().Result Then
                                Console.WriteLine("Session created")
                            Else
                                Console.WriteLine("Create session failed")
                            End If
                        End If
                    Next

                    Environment.Exit(0)
                Case Else
                    GoTo mainMenu
            End Select
        Else
            GoTo mainMenu
        End If

Get_Error:
        Console.ReadLine()
    End Sub

    Public Async Sub VertusThread(Query As VertusQuery)
        While True
            Dim RND As New Random()
            Try
                Dim Bot As New VertusBot(Query, proxies)
                If Not Bot.HasError Then
                    Log.Show("Vertus", Query.Name, $"my ip '{Bot.IPAddress}'", ConsoleColor.White)
                    Log.Show("Vertus", Query.Name, $"synced successfully. B<{Math.Round(Bot.UserDetail.User.Balance / 1000000000000000000, 2)}> P<{Math.Round(Bot.UserDetail.User.ValuePerHour / 1000000000000000000, 2)}> FL<{Bot.UserDetail.User.Abilities.Farm.Level}> SL<{Bot.UserDetail.User.Abilities.Storage.Level}> PL<{Bot.UserDetail.User.Abilities.Population.Level}>", ConsoleColor.Blue)

                    If Query.GameCollect And Bot.UserDetail.User.VertStorage > 2000000000000000000 Then
                        Dim collect = Await Bot.VertusCollect()
                        If collect IsNot Nothing Then
                            Bot.UserDetail.User.Balance = collect.NewBalance
                            Log.Show("Vertus", Query.Name, $"storage collect successfully", ConsoleColor.Green)
                        Else
                            Log.Show("Vertus", Query.Name, $"storage collect failed", ConsoleColor.Red)
                        End If

                        Thread.Sleep(3000)
                    End If

                    If Query.DailyReward And Bot.UserDetail.User.DailyRewards.LastRewardClaimed.HasValue Then
                        If Bot.UserDetail.User.DailyRewards.LastRewardClaimed.Value.AddDays(1) < Date.Now Then
                            Dim reward = Await Bot.VertusDailyReward()
                            If reward IsNot Nothing Then
                                Bot.UserDetail.User.Balance = reward.Balance
                                Log.Show("Vertus", Query.Name, $"daily reward claimed", ConsoleColor.Green)
                            Else
                                Log.Show("Vertus", Query.Name, $"claim daily reward failed", ConsoleColor.Red)
                            End If

                            Thread.Sleep(3000)
                        End If
                    End If

                    If Query.DailyCode And Bot.UserDetail.User.DailyCode.Code = "" Then
                        Dim answer = Await Bot.VertusAnswer()
                        If answer IsNot Nothing Then
                            If answer.Expire.ToLocalTime > Date.Now Then
                                Dim code = Await Bot.VertusCodes(answer.Code)
                                If code IsNot Nothing Then
                                    If code.IsValid Then
                                        Bot.UserDetail.User.Balance = code.NewBalance
                                        Log.Show("Vertus", Query.Name, $"daily code claimed", ConsoleColor.Green)
                                    Else
                                        Log.Show("Vertus", Query.Name, $"claim daily code failed", ConsoleColor.Red)
                                    End If
                                End If

                                Thread.Sleep(3000)
                            End If
                        End If
                    End If

                    If Query.Mission Then
                        Dim missions = Await Bot.VertusMissions()
                        If missions IsNot Nothing Then
                            For Each mission In missions.Sponsors2.Where(Function(x) x.IsActive And Not x.IsCompleted And Not x.IsOnlyAdmin And x.Resource = "ADSGRAM" And x.Completion < x.MaxCompletion)
                                Dim check = Await Bot.VertusCheckAdsgram()
                                If check Then
                                    Thread.Sleep(20000)
                                    Dim complete = Await Bot.VertusCompleteAdsgram()
                                    If complete IsNot Nothing Then
                                        If complete.IsSuccess Then
                                            Bot.UserDetail.User.Balance = complete.NewBalance
                                            Log.Show("Vertus", Query.Name, $"mission '{mission.Title}' completed", ConsoleColor.Green)
                                        Else
                                            Log.Show("Vertus", Query.Name, $"complete mission '{mission.Title}' failed", ConsoleColor.Red)
                                        End If
                                    End If
                                End If

                                Dim eachmissionRND As Integer = RND.Next(Query.MissionSleep(0), Query.MissionSleep(1))
                                Thread.Sleep(eachmissionRND * 1000)
                            Next
                        End If
                    End If

                    If Query.UpgradeLevel Then
                        If Bot.UserDetail.User.Abilities.Farm.NextLevel IsNot Nothing Then
                            Dim upgrade = Await Bot.VertusUpgrade("farm")
                            If upgrade IsNot Nothing Then
                                If upgrade.Success Then
                                    Bot.UserDetail.User.Balance = upgrade.NewBalance
                                    Bot.UserDetail.User.Abilities = upgrade.Abilities
                                    Log.Show("Vertus", Query.Name, $"farm upgraded", ConsoleColor.Green)
                                Else
                                    Log.Show("Vertus", Query.Name, $"upgrade farm failed", ConsoleColor.Red)
                                End If
                            End If

                            Thread.Sleep(3000)
                        End If

                        If Bot.UserDetail.User.Abilities.Storage.NextLevel IsNot Nothing Then
                            Dim upgrade = Await Bot.VertusUpgrade("storage")
                            If upgrade IsNot Nothing Then
                                If upgrade.Success Then
                                    Bot.UserDetail.User.Balance = upgrade.NewBalance
                                    Bot.UserDetail.User.Abilities = upgrade.Abilities
                                    Log.Show("Vertus", Query.Name, $"storage upgraded", ConsoleColor.Green)
                                Else
                                    Log.Show("Vertus", Query.Name, $"upgrade storage failed", ConsoleColor.Red)
                                End If
                            End If

                            Thread.Sleep(3000)
                        End If

                        If Bot.UserDetail.User.Abilities.Population.NextLevel IsNot Nothing Then
                            Dim upgrade = Await Bot.VertusUpgrade("population")
                            If upgrade IsNot Nothing Then
                                If upgrade.Success Then
                                    Bot.UserDetail.User.Balance = upgrade.NewBalance
                                    Bot.UserDetail.User.Abilities = upgrade.Abilities
                                    Log.Show("Vertus", Query.Name, $"population upgraded", ConsoleColor.Green)
                                Else
                                    Log.Show("Vertus", Query.Name, $"upgrade population failed", ConsoleColor.Red)
                                End If
                            End If

                            Thread.Sleep(3000)
                        End If
                    End If

                    If Query.UpgradeCard Then
                        Dim cards = Await Bot.VertusCards()
                        If cards IsNot Nothing Then
                            For Each card In cards.Where(Function(x) x.IsUpgradable And Not x.IsLocked And Not x.IsTon And x.Cost < (Bot.UserDetail.User.Balance / 50)).OrderBy(Function(x) x.Cost)
                                Dim upgrade = Await Bot.VertusUpgradeCard(card.Id)
                                If upgrade IsNot Nothing Then
                                    If upgrade.IsSuccess Then
                                        Bot.UserDetail.User.Balance = upgrade.Balance
                                        Bot.UserDetail.User.ValuePerHour = upgrade.NewValuePerHour
                                        Log.Show("Vertus", Query.Name, $"card '{card.CardName}' upgraded", ConsoleColor.Green)
                                    Else
                                        Log.Show("Vertus", Query.Name, $"card '{card.CardName}' upgrade failed", ConsoleColor.Red)
                                    End If

                                    Dim eachcardRND As Integer = RND.Next(Query.UpgradeCardSleep(0), Query.UpgradeCardSleep(1))
                                    Thread.Sleep(eachcardRND * 1000)
                                End If
                            Next
                        End If
                    End If

                    Log.Show("Vertus", Query.Name, $"B<{Math.Round(Bot.UserDetail.User.Balance / 1000000000000000000, 2)}> P<{Math.Round(Bot.UserDetail.User.ValuePerHour / 1000000000000000000, 2)}> FL<{Bot.UserDetail.User.Abilities.Farm.Level}> SL<{Bot.UserDetail.User.Abilities.Storage.Level}> PL<{Bot.UserDetail.User.Abilities.Population.Level}>", ConsoleColor.Blue)
                Else
                    Log.Show("Vertus", Query.Name, $"{Bot.ErrorMessage}", ConsoleColor.Red)
                End If
            Catch ex As Exception
                Log.Show("Vertus", Query.Name, $"Error: {ex.Message}", ConsoleColor.Red)
            End Try

            Dim syncRND As Integer = 0
            If Date.Now.Hour < 8 Then
                syncRND = RND.Next(Query.NightSleep(0), Query.NightSleep(1))
            Else
                syncRND = RND.Next(Query.DaySleep(0), Query.DaySleep(1))
            End If
            Log.Show("Vertus", Query.Name, $"sync sleep '{Int(syncRND / 3600)}h {Int((syncRND Mod 3600) / 60)}m {syncRND Mod 60}s'", ConsoleColor.Yellow)
            Thread.Sleep(syncRND * 1000)
        End While
    End Sub
End Module
