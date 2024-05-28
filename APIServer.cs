﻿using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using api;
using api2018;
using api2017;
using Newtonsoft.Json;
using vaultgamesesh;
using System.Collections.Generic;
using api2019;
using static api2019.AccountAuth;
using System.Security.AccessControl;
using start;

namespace server
{
	internal class APIServer
	{
		public APIServer()
		{
			try
			{
				Console.WriteLine("[APIServer.cs] has started.");
				new Thread(new ThreadStart(this.StartListen)).Start();
			}
			catch (Exception ex)
			{
				Console.WriteLine("An Exception Occurred while Listening :" + ex.ToString());
			}
		}
		private void StartListen()
		{
			try
			{
				
				this.listener.Prefixes.Add("http://localhost:" + start.Program.version + "/");
				{
					for (; ; )
					{
						this.listener.Start();
						Console.WriteLine("[APIServer.cs] is listening.");
						HttpListenerContext context = this.listener.GetContext();
						HttpListenerRequest request = context.Request;
						HttpListenerResponse response = context.Response;
						List<byte> list = new List<byte>();
						string rawUrl = request.RawUrl;
						string Url = "";
						byte[] bytes = null;
						string signature = request.Headers.Get("X-RNSIG");
						if (rawUrl.StartsWith("/api/"))
						{
							Url = rawUrl.Remove(0, 5);
						}
						if (!(Url == ""))
						{
							Console.WriteLine("API Requested: " + Url);
						}
						else
						{
							Console.WriteLine("API Requested (rawUrl): " + rawUrl);
						}
						string text;
						string s = "";
						using (StreamReader streamReader = new StreamReader(request.InputStream, request.ContentEncoding))
						{
							text = streamReader.ReadToEnd();
						}
						Console.WriteLine("API Data: " + text);
						if (Url.StartsWith("versioncheck"))
						{
							CachedVersionMonth = 05;
							
							s = VersionCheckResponse2;
						}
						if (Url == ("config/v2"))
						{
							s = Config2.GetDebugConfig();
						}
						if (Url == "platformlogin/v1/getcachedlogins")
						{
							s = getcachedlogins.GetDebugLogin(ulong.Parse(text.Remove(0, 32)), ulong.Parse(text.Remove(0, 22)));
							CachedPlayerID = ulong.Parse(text.Remove(0, 32));
							CachedPlatformID = ulong.Parse(text.Remove(0, 22));
							File.WriteAllText("SaveData\\Profile\\userid.txt", Convert.ToString(CachedPlayerID));
							
						}
						if (Url == "platformlogin/v1/loginaccount")
						{
							s = logincached.loginCache(CachedPlayerID, CachedPlatformID);
						}
						if (Url == "platformlogin/v1/createaccount")
						{
							s = logincached.loginCache(CachedPlayerID, CachedPlatformID);
						}
						if (Url == "platformlogin/v1/logincached")
						{
							s = logincached.loginCache(CachedPlayerID, CachedPlatformID);
						}
						if (Url == "relationships/v1/bulkignoreplatformusers")
						{
							s = BlankResponse;
						}
                        if (Url.StartsWith("roomkeys/"))
                        {
                            s = BracketResponse;
                        }
                        if (rawUrl.StartsWith("/account/bulk"))
                        {
                            s = AccountAuth.GetAccountsBulk();
                        }
                        if (rawUrl.StartsWith("/account/me"))
                        {
                            s = JsonConvert.SerializeObject(JsonConvert.DeserializeObject<List<Account>>(AccountAuth.GetAccountsBulk())[0]);

                        }
                        if (rawUrl.StartsWith("/player/login"))
                        {
                            s = AccountAuth.ConnectToken();
                            
                        }
                        if (Url.StartsWith("players/v1/progression/"))
                        {
                            s = AccountAuth.GetLevel();
                            
                        }
                        if (Url.StartsWith("playerReputation/v1/"))
                        {
                            s = AccountAuth.GetRep();
                            
                        }
                        if (Url == "players/v1/list")
						{
							s = BracketResponse;
						}
						if (Url == "config/v1/amplitude")
						{
							s = Amplitude.amplitude();
						}
						if (Url == "images/v2/named")
						{
							s = ImagesV2Named;
						}
						if (Url == "PlayerReporting/v1/moderationBlockDetails")
						{
							s = ModerationBlockDetails;
							
						}
						if (Url == "//api/chat/v2/myChats?mode=0&count=50")
						{
							s = BracketResponse;
						}
						if (Url == "messages/v2/get")
						{
							s = BracketResponse;
						}
						if (Url == "relationships/v2/get")
						{
							s = BracketResponse;
						}
						if (Url == "gameconfigs/v1/all")
						{
							s = File.ReadAllText("SaveData\\gameconfigs.txt");
						}
						if (Url.StartsWith("storefronts/v3/giftdropstore"))
						{
							
							s = BracketResponse;
							
						}
						if (Url.StartsWith("storefronts/v3/balance/"))
						{
							s = BracketResponse;
						}
						if (Url == "avatar/v2")
						{
							s = File.ReadAllText("SaveData\\avatar.txt");
						}
						if (Url == "avatar/v2/saved")
						{
							s = BracketResponse;
						}
						if (Url == "avatar/v2/set")
						{
							//for later 2018 builds compatibility
							if (!(text.Contains("FaceFeatures")))
							{
								string postdatacache = text;
								text = postdatacache.Remove(postdatacache.Length - 1, 1) + File.ReadAllText("SaveData\\App\\facefeaturesadd.txt");
							}
							File.WriteAllText("SaveData\\avatar.txt", text);
						}
						if (Url == "settings/v2/")
						{
							s = File.ReadAllText("SaveData\\settings.txt");
						}
						if (Url == "settings/v2/set")
						{
							Settings.SetPlayerSettings(text);
						}
                        if (rawUrl.Contains("/club/"))
                        {
                            s = BracketResponse;
                        }
                        if (rawUrl.Contains("/api/storefronts/v3"))
                        {
                            s = BracketResponse;
                        }
                        if (rawUrl.Contains("/thread"))
                        {
                            s = BracketResponse;
                        }

                        if (rawUrl.Contains("player/heartbeat"))
                        {
                            s = "{\"playerId\":7383744,\"statusVisibility\":0,\"deviceClass\":0,\"roomInstance\":{\"roomInstanceId\":505995663,\"roomId\":1,\"subRoomId\":1,\"location\":\"76d98498-60a1-430c-ab76-b54a29b7a163\",\"photonRegionId\":\"us\",\"photonRoomId\":\"1Re0.5.1born76376384\",\"name\":\"^DormRoom\",\"maxCapacity\":20,\"dataBlob\":\"\",\"isFull\":false,\"isPrivate\":true,\"isInProgress\":false}}";
                        }
                        ///player/heartbeat
                        // 
                        if (rawUrl == "//api/chat/v2/myChats?mode=0&count=50")
						{
							s = BracketResponse;
						}
						if (Url == "playersubscriptions/v1/my")
						{
							s = BracketResponse;
						}
						if (Url == "avatar/v3/items")
						{

							s = File.ReadAllText("SaveData\\avataritems2.txt");
							
						}
                        if (Url == "avatar/v4/items")
                        {

                            s = File.ReadAllText("SaveData\\avataritems2.txt");


                        }
                        if (rawUrl.Contains("quickPlay/v1/getandclear"))
                        {
                            s = JsonConvert.SerializeObject((object)new QuickPlayResponseDTO()
                            {
                                TargetPlayerId = int.Parse(File.ReadAllText(Program.ProfilePath + "\\userid.txt")),
                                RoomName = (string)null,
                                ActionCode = (string)null
                            });
                        }
                        if (Url == "equipment/v1/getUnlocked")
						{
							s = File.ReadAllText("SaveData\\equipment.txt");
						}
						if (Url == "avatar/v1/saved")
						{
							s = BracketResponse;
						}
						if (Url == "consumables/v1/getUnlocked")
						{
							
							s = File.ReadAllText("SaveData\\consumables.txt");
							
						}
						if (Url == "avatar/v2/gifts")
						{
							s = BracketResponse;
						}
						if (Url == "storefronts/v2/2")
						{
							s = BlankResponse;
						}
						if (Url == "storefronts/v1/allGiftDrops/2")
						{
							s = BracketResponse;
						}
						if (Url == "objectives/v1/myprogress")
						{
							s = JsonConvert.SerializeObject(new Objective2018());
						}
						if (Url == "rooms/v1/myrooms")
						{
							s = File.ReadAllText("SaveData\\myrooms.txt");
						}
						if (Url == "rooms/v2/myrooms")
						{
							s = BracketResponse;
						}
						if (Url == "rooms/v2/baserooms")
						{
							s = File.ReadAllText("SaveData\\baserooms.txt");
						}
						if (Url == "rooms/v1/mybookmarkedrooms")
						{
							s = BracketResponse;
						}
						if (Url == "rooms/v1/myRecent?skip=0&take=10")
						{
							s = BracketResponse;
						}
						if (Url == "events/v3/list")
						{
							s = Events.list();
						}
						if (Url == "playerevents/v1/all")
						{
							s = PlayerEventsResponse;
						}
						if (Url == "activities/charades/v1/words")
						{
							s = Activities.Charades.words();
						}
						if (Url == "gamesessions/v2/joinrandom")
						{
							s = gamesesh.GameSessions.JoinRandom(text);
						}
						if (Url == "gamesessions/v2/create")
						{
							s = gamesesh.GameSessions.Create(text);
						}
						if (Url == "gamesessions/v3/joinroom")
						{
							s = JsonConvert.SerializeObject(c000041.m000030(text));
						}
						if (rawUrl == "//api/sanitize/v1/isPure")
						{
							s = "{\"IsPure\":true}";
						}
                        if (Url == "avatar/v1/defaultunlocked")
                        {
                            s = BracketResponse;
                        }
                        if (Url == "avatar/v3/saved")
						{
							s = BracketResponse;
						}
						if (Url == "checklist/v1/current")
						{
							s = ChecklistV1Current;
						}
						if (Url == "presence/v1/setplayertype")
						{
							s = BracketResponse;
						}
						if (Url == "challenge/v1/getCurrent")
						{
							s = ChallengesV1GetCurrent;
						}
						if (Url == "rooms/v1/featuredRoomGroup")
						{
							s = BracketResponse;
						}
						if (Url == "rooms/v1/clone")
						{
							s = JsonConvert.SerializeObject(c000099.m00000a(text));
						}
						if (Url.StartsWith("rooms/v2/saveData"))
						{
                            string text26 = "5GDNL91ZY43PXN2YJENTBL";
                            string path = c000004.m000007() + c000041.f000043.Room.Name;
                            File.WriteAllBytes(string.Concat(new string[]
                            {
								c000004.m000007(),
								c000041.f000043.Room.Name,
								"\\room\\",
								text26,
								".room"
                            }), c0000a5.m00005d(list.ToArray(), "data.dat"));
                            c000041.f000043.Scenes[0].DataBlobName = text26 + ".room";
                            c000041.f000043.Scenes[0].DataModifiedAt = DateTime.Now;
                            File.WriteAllText(c000004.m000007() + c000041.f000043.Room.Name + "\\RoomDetails.json", JsonConvert.SerializeObject(c000041.f000043));
                            s = JsonConvert.SerializeObject(c00005d.m000035());
                        }
						if (Url == "presence/v3/heartbeat")
						{
							
							s = JsonConvert.SerializeObject(Notification2018.Reponse.createResponse(4, c000020.m000027()));
						}
                        if (Url == "PlayerReporting/v1/hile")
                        {
                            s = "{\"Message\":\"\",\"Type\":0}";
                        }
                        if (rawUrl == "config/LoadingScreenTipData")
                        {
							s = BracketResponse;
                        }
                        ///config/LoadingScreenTipData
                        if (Url == "rooms/v1/featuredRoomGroup")
						{
							s = new WebClient().DownloadString("https://raw.githubusercontent.com/wiiboi69/Rec_rewild/master/Update/dormslideshow.txt");
						}
						if (Url.StartsWith("rooms/v1/hot"))
						{
							s = new WebClient().DownloadString("https://raw.githubusercontent.com/wiiboi69/Rec_rewild/master/Update/hotrooms.txt");
						}
						if (Url.StartsWith("rooms/v2/instancedetails"))
						{
							s = BracketResponse;
						}
						if (Url.StartsWith("rooms/v2/search?value="))
						{
							CustomRooms.RoomGet(Url.Remove(0, 22));
						}
						if (Url == "rooms/v4/details/29")
						{
							s = File.ReadAllText("SaveData\\Rooms\\Downloaded\\RoomDetails.json");
							Thread.Sleep(100);
						}
						else if (Url.StartsWith("rooms/v4/details"))
						{
							s = JsonConvert.SerializeObject(c00005d.m000023(Convert.ToInt32(Url.Remove(0, 17))));
						}
						if (Url == "images/v1/slideshow")
						{
							s = new WebClient().DownloadString("https://raw.githubusercontent.com/wiiboi69/Rec_rewild/master/Update/rcslideshow.txt");
						}

                        if (s.Length > 400)
                        {
                            Console.WriteLine("api Response: " + s.Length);
                        }
                        else
                        {
                            Console.WriteLine("api Response: " + s);
                        }
                        bytes = Encoding.UTF8.GetBytes(s);
						response.ContentLength64 = (long)bytes.Length;
						Stream outputStream = response.OutputStream;
						outputStream.Write(bytes, 0, bytes.Length);
						Thread.Sleep(200);
						outputStream.Close();
						this.listener.Stop();

					}
				}
			}
			catch (Exception ex4)
			{
				Console.WriteLine(ex4);
				File.WriteAllText("crashdump.txt", Convert.ToString(ex4));
				this.listener.Close();
				new APIServer();
			}
		}

        public class QuickPlayResponseDTO
        {
            public long? TargetPlayerId { get; set; }

            public string RoomName { get; set; }

            public string ActionCode { get; set; }
        }

        public static ulong CachedPlayerID = ulong.Parse(File.ReadAllText("SaveData\\Profile\\userid.txt"));
		public static ulong CachedPlatformID = 10000;
		public static int CachedVersionMonth = 01;

		public static string BlankResponse = "";
		public static string BracketResponse = "[]";

		public static string PlayerEventsResponse = "{\"Created\":[],\"Responses\":[]}";
		public static string VersionCheckResponse2 = "{\"VersionStatus\":0}";
		public static string VersionCheckResponse = "{\"ValidVersion\":true}";
		public static string ModerationBlockDetails = "{\"ReportCategory\":0,\"Duration\":0,\"GameSessionId\":0,\"Message\":\"\"}";
		public static string ImagesV2Named = "[{\"FriendlyImageName\":\"DormRoomBucket\",\"ImageName\":\"DormRoomBucket\",\"StartTime\":\"2021-12-27T21:27:38.1880175-08:00\",\"EndTime\":\"2025-12-27T21:27:38.1880399-08:00\"}";
		public static string ChallengesV1GetCurrent = "{\"Success\":true,\"Message\":\"OpenRec\"}";
		public static string ChecklistV1Current = "[{\"Order\":0,\"Objective\":3000,\"Count\":3,\"CreditAmount\":100},{\"Order\":1,\"Objective\":3001,\"Count\":3,\"CreditAmount\":100},{\"Order\":2,\"Objective\":3002,\"Count\":3,\"CreditAmount\":100}]";

		public static string Banned = "{\"ReportCategory\":1,\"Duration\":10000000000000000,\"GameSessionId\":100,\"Message\":\"You have been banned. You are probably a little kid and are now whining at your VR headset. If you aren't a little kid, DM me to appeal.\"}";

		private HttpListener listener = new HttpListener(); 
	}
}
