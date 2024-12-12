Imports System.Text.Json.Serialization

Public Class VertusQuery
    Public Property Index As Long
    Public Property Name As String
    Public Property API_ID As String
    Public Property API_HASH As String
    Public Property Phone As String
    Public Property Auth As String
    Public Property Active As Boolean
    Public Property GameCollect As Boolean
    Public Property DailyReward As Boolean
    Public Property FriendBonus As Boolean
    Public Property DailyCode As Boolean
    Public Property UpgradeLevel As Boolean
    Public Property UpgradeCard As Boolean
    Public Property UpgradeCardSleep As Integer()
    Public Property Mission As Boolean
    Public Property MissionSleep As Integer()
    Public Property DaySleep As Integer()
    Public Property NightSleep As Integer()
End Class

Public Class VertusUserDataResponse
    <JsonPropertyName("isValid")>
    Public Property IsValid As Boolean
    <JsonPropertyName("user")>
    Public Property User As VertusUserDataUser
End Class

Public Class VertusUserDataUser
    <JsonPropertyName("telegramId")>
    Public Property TelegramId As Long
    <JsonPropertyName("walletAddress")>
    Public Property WalletAddress As String
    <JsonPropertyName("bounceableWallet")>
    Public Property BounceableWallet As String
    <JsonPropertyName("balance")>
    Public Property Balance As Double
    <JsonPropertyName("storage")>
    Public Property Storage As Double
    <JsonPropertyName("activated")>
    Public Property Activated As Boolean
    <JsonPropertyName("abilities")>
    Public Property Abilities As VertusUserDataUserAbilities
    <JsonPropertyName("groupId")>
    Public Property GroupId As String
    <JsonPropertyName("vertStorage")>
    Public Property VertStorage As Double
    <JsonPropertyName("isIpSaved")>
    Public Property IsIpSaved As Boolean
    <JsonPropertyName("dailyRewards")>
    Public Property DailyRewards As VertusUserDataUserDailyRewards
    <JsonPropertyName("dailyCode")>
    Public Property DailyCode As VertusUserDataUserDailyCode
    <JsonPropertyName("lastOnline")>
    Public Property LastOnline As DateTime
    <JsonPropertyName("valuePerHour")>
    Public Property ValuePerHour As Double
    <JsonPropertyName("earnedOffline")>
    Public Property EarnedOffline As Double
End Class

Public Class VertusUserDataUserAbilities
    <JsonPropertyName("farm")>
    Public Property Farm As VertusUserDataUserAbilitiesItem
    <JsonPropertyName("storage")>
    Public Property Storage As VertusUserDataUserAbilitiesItem
    <JsonPropertyName("population")>
    Public Property Population As VertusUserDataUserAbilitiesItem
End Class

Public Class VertusUserDataUserAbilitiesItem
    <JsonPropertyName("value")>
    Public Property [Value] As Double
    <JsonPropertyName("level")>
    Public Property Level As Integer
    <JsonPropertyName("title")>
    Public Property Title As String
    <JsonPropertyName("priceToLevelUp")>
    Public Property PriceToLevelUp As Double?
    <JsonPropertyName("nextLevel")>
    Public Property NextLevel As VertusUserDataUserAbilitiesItemNextLevel
End Class

Public Class VertusUserDataUserAbilitiesItemNextLevel
    <JsonPropertyName("level")>
    Public Property Level As Integer
End Class

Public Class VertusUserDataUserDailyRewards
    <JsonPropertyName("consecutiveDays")>
    Public Property ConsecutiveDays As Integer
    <JsonPropertyName("lastRewardClaimed")>
    Public Property LastRewardClaimed As DateTime?
End Class

Public Class VertusUserDataUserDailyCode
    <JsonPropertyName("validDate")>
    Public Property ValidDate As DateTime?
    <JsonPropertyName("code")>
    Public Property Code As String
End Class

Public Class VertusCheckResponse
    <JsonPropertyName("isSuccess")>
    Public Property IsSuccess As Boolean
End Class

Public Class VertusPingResponse
    <JsonPropertyName("message")>
    Public Property Message As String
End Class

Public Class VertusCards
    Public Property IsTon As Boolean
    Public Property Id As String
    Public Property CardName As String
    Public Property [Type] As String
    Public Property Level As Integer
    Public Property Cost As Double
    Public Property [Value] As Double
    Public Property IsUpgradable As Boolean
    Public Property NextValue As Double
    Public Property IsLocked As Boolean
End Class

Public Class VertusCardsResponse
    <JsonPropertyName("economyCards")>
    Public Property EconomyCards As List(Of VertusCardsItem)
    <JsonPropertyName("militaryCards")>
    Public Property MilitaryCards As List(Of VertusCardsItem)
    <JsonPropertyName("scienceCards")>
    Public Property ScienceCards As List(Of VertusCardsItem)
End Class

Public Class VertusCardsItem
    <JsonPropertyName("isTon")>
    Public Property IsTon As Boolean
    <JsonPropertyName("_id")>
    Public Property Id As String
    <JsonPropertyName("cardName")>
    Public Property CardName As String
    <JsonPropertyName("levels")>
    Public Property Levels As List(Of VertusCardsItemLevels)
    <JsonPropertyName("type")>
    Public Property [Type] As String
    <JsonPropertyName("currentLevel")>
    Public Property CurrentLevel As Integer
    <JsonPropertyName("currentValue")>
    Public Property CurrentValue As Double
    <JsonPropertyName("isUpgradable")>
    Public Property IsUpgradable As Boolean
    <JsonPropertyName("nextValue")>
    Public Property NextValue As Double
    <JsonPropertyName("isLocked")>
    Public Property IsLocked As Boolean
End Class

Public Class VertusCardsItemLevels
    <JsonPropertyName("_id")>
    Public Property Id As String
    <JsonPropertyName("cost")>
    Public Property Cost As Double
    <JsonPropertyName("value")>
    Public Property [Value] As Double
End Class

Public Class VertusUpgradeCardRequest
    <JsonPropertyName("cardId")>
    Public Property CardId As String
End Class

Public Class VertusUpgradeCardResponse
    <JsonPropertyName("isSuccess")>
    Public Property IsSuccess As Boolean
    <JsonPropertyName("balance")>
    Public Property Balance As Double
    <JsonPropertyName("newValuePerHour")>
    Public Property NewValuePerHour As Double
End Class

Public Class VertusMissionsRequest
    <JsonPropertyName("isPremium")>
    Public Property IsPremium As Boolean
    <JsonPropertyName("languageCode")>
    Public Property LanguageCode As String
End Class

Public Class VertusMissionsResponse
    <JsonPropertyName("notCompleted")>
    Public Property NotCompleted As Integer
    <JsonPropertyName("sponsorNotCompleted")>
    Public Property SponsorNotCompleted As Integer
    <JsonPropertyName("groups")>
    Public Property Groups As List(Of VertusMissionsGroups)
    <JsonPropertyName("sponsors2")>
    Public Property Sponsors2 As List(Of VertusMissionsGroupsMissions)
End Class

Public Class VertusMissionsGroups
    <JsonPropertyName("title")>
    Public Property Title As String
    <JsonPropertyName("missions")>
    Public Property Missions As List(Of List(Of VertusMissionsGroupsMissions))
End Class

Public Class VertusMissionsSponsors2
    <JsonPropertyName("title")>
    Public Property Title As String
    <JsonPropertyName("missions")>
    Public Property Missions As List(Of List(Of VertusMissionsGroupsMissions))
End Class

Public Class VertusMissionsGroupsMissions
    <JsonPropertyName("isCIS")>
    Public Property IsCIS As Boolean
    <JsonPropertyName("isWeb3User")>
    Public Property IsWeb3User As Boolean
    <JsonPropertyName("_id")>
    Public Property Id As String
    <JsonPropertyName("title")>
    Public Property Title As String
    '<JsonPropertyName("reward")>
    'Public Property Reward As Long
    <JsonPropertyName("type")>
    Public Property [Type] As String
    <JsonPropertyName("resource")>
    Public Property Resource As String
    <JsonPropertyName("isActive")>
    Public Property IsActive As Boolean
    <JsonPropertyName("isOnlyAdmin")>
    Public Property IsOnlyAdmin As Boolean
    <JsonPropertyName("isCompleted")>
    Public Property IsCompleted As Boolean
    <JsonPropertyName("completion")>
    Public Property Completion As Integer
    <JsonPropertyName("maxCompletion")>
    Public Property MaxCompletion As Integer
End Class

Public Class VertusMissionsCompleteRequest
    <JsonPropertyName("missionId")>
    Public Property MissionId As String
End Class

Public Class VertusMissionsCompleteResponse
    <JsonPropertyName("newBalance")>
    Public Property NewBalance As Double
    <JsonPropertyName("isCompleted")>
    Public Property IsCompleted As Boolean
End Class

Public Class VertusCollectResponse
    <JsonPropertyName("newBalance")>
    Public Property NewBalance As Double
End Class

Public Class VertusAdsgramCompleteResponse
    <JsonPropertyName("isSuccess")>
    Public Property IsSuccess As Boolean
    <JsonPropertyName("newBalance")>
    Public Property NewBalance As Double
End Class

Public Class VertusDailyRewardResponse
    <JsonPropertyName("success")>
    Public Property Success As Boolean
    <JsonPropertyName("balance")>
    Public Property Balance As Double
End Class

Public Class VertusCountriesResponse
    <JsonPropertyName("data")>
    Public Property Data As VertusCountriesData
    <JsonPropertyName("isSuccess")>
    Public Property IsSuccess As Boolean
End Class

Public Class VertusCountriesData
    <JsonPropertyName("_id")>
    Public Property Id As String
    <JsonPropertyName("title")>
    Public Property Title As String
    <JsonPropertyName("balance")>
    Public Property Balance As Double
    <JsonPropertyName("totalMembers")>
    Public Property TotalMembers As Integer
    <JsonPropertyName("rank")>
    Public Property Rank As Integer
End Class

Public Class VertusUpgradeRequest
    <JsonPropertyName("upgrade")>
    Public Property Upgrade As String
End Class

Public Class VertusUpgradeResponse
    <JsonPropertyName("success")>
    Public Property Success As Boolean
    <JsonPropertyName("abilities")>
    Public Property Abilities As VertusUserDataUserAbilities
    <JsonPropertyName("newBalance")>
    Public Property NewBalance As Double
End Class

Public Class VertusCodesRequest
    <JsonPropertyName("code")>
    Public Property Code As String
End Class

Public Class VertusCodesResponse
    <JsonPropertyName("isValid")>
    Public Property IsValid As Boolean
    <JsonPropertyName("newBalance")>
    Public Property NewBalance As Double
End Class

Public Class VertusAnswerResponse
    <JsonPropertyName("expire")>
    Public Property Expire As DateTime
    <JsonPropertyName("code")>
    Public Property Code As String
End Class

Public Class Proxy
    Public Property Index As Integer
    Public Property Proxy As String
End Class

Public Class httpbin
    <JsonPropertyName("origin")>
    Public Property Origin As String
End Class