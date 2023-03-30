using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Terraria;
using TShockAPI;

namespace Hunting
{
	// Token: 0x02000005 RID: 5
	internal static class Utils
	{
		// Token: 0x0600003C RID: 60 RVA: 0x000045D4 File Offset: 0x000027D4
		public static void Jump()
		{
			foreach (KeyValuePair<int, bool> keyValuePair in Structs.Values.playing)
			{
				bool value = keyValuePair.Value;
				if (value)
				{
					TShock.Players[keyValuePair.Key].TPlayer.ClearBuff(211);
				}
			}
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00004650 File Offset: 0x00002850
		public static void JoinLoop()
		{
			Config config = Config.GetConfig();
			int num = 0;
			TShock.Utils.Broadcast(string.Format("游戏将在{0}s后开始，请未加入游戏的玩家", config.MaxJoinSec) + "尽快输入/join加入游戏，并请各位玩家确认自己的队伍，游戏开始后将不能更换队伍", Color.Yellow);
			for (int i = 0; i < config.MaxJoinSec; i++)
			{
				Thread.Sleep(1000);
				num++;
				bool flag = config.MaxJoinSec - i <= 10;
				if (flag)
				{
					TShock.Utils.Broadcast(string.Format("游戏倒计时:{0}", config.MaxJoinSec - i), Color.Red);
				}
				else
				{
					bool flag2 = num == 10;
					if (flag2)
					{
						num = 0;
						TShock.Utils.Broadcast(string.Format("游戏将在{0}s后开始，请未加入游戏的玩家", config.MaxJoinSec - i) + "尽快输入/join加入游戏，并请各位玩家确认自己的队伍，游戏开始后将不能更换队伍", Color.Yellow);
					}
				}
			}
			Utils.Initialize();
			TShock.Utils.Broadcast("猎杀时刻！", Color.Red);
			Structs.Values.Game = Structs.Status.playing;
			Structs.Values.JoinThread = null;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00004768 File Offset: 0x00002968
		public static void PlayerKilled(int killed, int killer)
		{
			bool flag = killed != killer;
			if (flag)
			{
				bool flag2 = Utils.IsFalled(killed);
				if (flag2)
				{
					Structs.Values.Falled.Remove(killed);
				}
				Structs.Values.Killed.Add(killed);
				Structs.Values.playing[killed] = false;
				Dictionary<string, int> grades = Structs.Values.Grades;
				string playerName = Utils.GetPlayerName(killer);
				int num = grades[playerName];
				grades[playerName] = num + 1;
				TShock.Utils.Broadcast(string.Concat(new string[]
				{
					Utils.GetPlayerName(killed),
					"已被",
					Utils.GetPlayerName(killer),
					"击杀,",
					Utils.GetPlayerName(killer),
					string.Format("总分+1，{0}当前总分:{1}", Utils.GetPlayerName(killer), Structs.Values.Grades[Utils.GetPlayerName(killer)])
				}), Utils.GetTeamColor(Structs.Values.Teams[killer]));
				TShock.Players[killed].SendErrorMessage("您已出局，可输入/join 玩家名，传送至某一玩家进行观战");
			}
			else
			{
				bool flag3 = Utils.IsFalled(killed);
				if (flag3)
				{
					Structs.Values.Falled.Remove(killed);
				}
				Structs.Values.Killed.Add(killed);
				Structs.Values.playing[killed] = false;
				TShock.Utils.Broadcast(Utils.GetPlayerName(killed) + "已出局", Utils.GetTeamColor(Structs.Values.Teams[killed]));
				TShock.Players[killed].SendErrorMessage("您已出局，可输入/join 玩家名，传送至某一玩家进行观战");
			}
			Utils.CountGrades();
			int tileX = TShock.Players[killed].TileX;
			int tileY = TShock.Players[killed].TileY;
			TShock.Players[killed].Spawn(0, null);
			Structs.Item[] playerBank = Utils.GetPlayerBank(killed);
			Utils.SetPlayerInv(killed, Structs.Values.PlayerInvs[killed]);
			foreach (Structs.Item item in playerBank)
			{
				Utils.DropItem(item, tileX, tileY);
			}
		}

		// Token: 0x0600003F RID: 63 RVA: 0x00004954 File Offset: 0x00002B54
		public static void PlayerFalled(int killed, int killer)
		{
			bool flag = Structs.Values.Teams[killed] == Structs.Team.无 || Utils.GetTeammates(killed).Count == 0;
			if (flag)
			{
				Utils.PlayerKilled(killed, killer);
			}
			else
			{
				bool flag2 = killed != killer;
				if (flag2)
				{
					Structs.Values.Falled.Add(killed);
					Dictionary<string, int> grades = Structs.Values.Grades;
					string playerName = Utils.GetPlayerName(killer);
					int num = grades[playerName];
					grades[playerName] = num + 1;
					TShock.Utils.Broadcast(string.Concat(new string[]
					{
						Utils.GetPlayerName(killed),
						"已被",
						Utils.GetPlayerName(killer),
						"击倒,",
						Utils.GetPlayerName(killer),
						string.Format("总分+1，{0}当前总分:{1}", Utils.GetPlayerName(killer), Structs.Values.Grades[Utils.GetPlayerName(killer)])
					}), Utils.GetTeamColor(Structs.Values.Teams[killer]));
					foreach (int num2 in Utils.GetTeammates(killed))
					{
						TShock.Players[num2].SendErrorMessage("玩家" + Utils.GetPlayerName(killed) + "已倒地，可通过在其身边10格范围内停留10s将其扶起");
					}
				}
				else
				{
					Structs.Values.Falled.Add(killed);
					TShock.Utils.Broadcast(Utils.GetPlayerName(killed) + "已倒地", Utils.GetTeamColor(Structs.Values.Teams[killed]));
					foreach (int num3 in Utils.GetTeammates(killed))
					{
						TShock.Players[num3].SendErrorMessage("玩家" + Utils.GetPlayerName(killed) + "已倒地，可通过在其身边10格范围内停留10s将其扶起");
					}
				}
				Vector2 vector = Structs.Values.LastPosition[killed];
				TShock.Players[killed].Spawn(0, null);
				TShock.Players[killed].Teleport(vector.X, vector.Y, 1);
				ThreadPool.QueueUserWorkItem(new WaitCallback(Utils.PlayerFallLoop), killed);
			}
		}

		// Token: 0x06000040 RID: 64 RVA: 0x00004BAC File Offset: 0x00002DAC
		public static List<int> GetTeammates(int who)
		{
			Structs.Team team = Structs.Values.Teams[who];
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, Structs.Team> keyValuePair in Structs.Values.Teams)
			{
				bool flag = keyValuePair.Value == team && keyValuePair.Key != who && Utils.IsPlayer(keyValuePair.Key);
				if (flag)
				{
					list.Add(keyValuePair.Key);
				}
			}
			return list;
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00004C50 File Offset: 0x00002E50
		public static bool CanSave(int hurter, int saver)
		{
			TSPlayer tsplayer = TShock.Players[hurter];
			TSPlayer tsplayer2 = TShock.Players[saver];
			return tsplayer2.X <= tsplayer.X + 160f && tsplayer2.X >= tsplayer.X - 160f && tsplayer2.Y >= tsplayer.Y - 160f && tsplayer2.Y <= tsplayer.Y + 160f;
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00004CC8 File Offset: 0x00002EC8
		public static void PlayerFallLoop(object o)
		{
			int num = (int)o;
			TSPlayer tsplayer = TShock.Players[num];
			while (Utils.IsFalled(num) && Utils.IsPlayer(num))
			{
				Config config = Config.GetConfig();
				foreach (int num2 in config.FalledBuffs)
				{
					tsplayer.SetBuff(num2, 20, false);
				}
				Thread.Sleep(1000);
			}
		}

		// Token: 0x06000043 RID: 67 RVA: 0x00004D40 File Offset: 0x00002F40
		public static void Save(object o)
		{
			try
			{
				List<int> list = (List<int>)o;
				int num = list[0];
				TSPlayer tsplayer = TShock.Players[num];
				int num2 = list[1];
				TShock.Players[num2].SendSuccessMessage("正在救治玩家" + tsplayer.Name + ",10s后可将其扶起");
				tsplayer.SendSuccessMessage("您正在被" + Utils.GetPlayerName(num2) + "救治，10s后您将被扶起");
				Thread.Sleep(10000);
				Structs.Values.Falled.Remove(num);
				TShock.Utils.Broadcast(string.Concat(new string[]
				{
					Utils.GetPlayerName(num2),
					"成功将",
					tsplayer.Name,
					"救起，",
					Utils.GetPlayerName(num2),
					"总分+1,",
					Utils.GetPlayerName(num2),
					"总分：",
					string.Format("{0}", Structs.Values.Grades[Utils.GetPlayerName(num2)])
				}), Utils.GetTeamColor(Structs.Values.Teams[num2]));
				Dictionary<string, int> grades = Structs.Values.Grades;
				string playerName = Utils.GetPlayerName(num2);
				int num3 = grades[playerName];
				grades[playerName] = num3 + 1;
				Structs.Values.SavePlayerLoop.Remove(num2);
			}
			catch (Exception ex)
			{
				string text = "在调用方法‘save’时报错:";
				Exception ex2 = ex;
				Console.WriteLine(text + ((ex2 != null) ? ex2.ToString() : null));
			}
		}

		// Token: 0x06000044 RID: 68 RVA: 0x00004EC4 File Offset: 0x000030C4
		public static bool IsFalled(int who)
		{
			return Structs.Values.Falled.FindAll((int i) => i == who).Count != 0;
		}

		// Token: 0x06000045 RID: 69 RVA: 0x00004F04 File Offset: 0x00003104
		public static void SetTeam(int who, Structs.Team team)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(5);
					binaryWriter.Write(45);
					binaryWriter.Write((byte)who);
					binaryWriter.Write((byte)team);
				}
				TShock.Players[who].SendRawData(memoryStream.ToArray());
			}
		}

		// Token: 0x06000046 RID: 70 RVA: 0x00004F90 File Offset: 0x00003190
		public static bool IsKilled(int who)
		{
			return Structs.Values.Killed.FindAll((int i) => i == who).Count != 0;
		}

		// Token: 0x06000047 RID: 71 RVA: 0x00004FD0 File Offset: 0x000031D0
		public static Dictionary<string, int> RankPlayers()
		{
			return Structs.Values.Grades.OrderBy((KeyValuePair<string, int> o) => o.Value).ToDictionary((KeyValuePair<string, int> p) => p.Key, (KeyValuePair<string, int> o) => o.Value);
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00005050 File Offset: 0x00003250
		public static Dictionary<Structs.Team, int> RankTeams()
		{
			List<Structs.Team> teams = Utils.GetTeams();
			Dictionary<Structs.Team, int> dictionary = new Dictionary<Structs.Team, int>();
			foreach (Structs.Team team in teams)
			{
				dictionary.Add(team, 0);
				foreach (KeyValuePair<string, int> keyValuePair in Utils.InTeamRank(team))
				{
					Dictionary<Structs.Team, int> dictionary2 = dictionary;
					Structs.Team team2 = team;
					dictionary2[team2] += keyValuePair.Value;
				}
			}
			return dictionary.OrderBy((KeyValuePair<Structs.Team, int> o) => o.Value).ToDictionary((KeyValuePair<Structs.Team, int> p) => p.Key, (KeyValuePair<Structs.Team, int> o) => o.Value);
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00005184 File Offset: 0x00003384
		public static Dictionary<string, int> InTeamRank(Structs.Team team)
		{
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			foreach (KeyValuePair<int, Structs.Team> keyValuePair in Structs.Values.Teams)
			{
				bool flag = keyValuePair.Value == team;
				if (flag)
				{
					dictionary.Add(Utils.GetPlayerName(keyValuePair.Key), Structs.Values.Grades[Utils.GetPlayerName(keyValuePair.Key)]);
				}
			}
			return dictionary.OrderBy((KeyValuePair<string, int> o) => o.Value).ToDictionary((KeyValuePair<string, int> p) => p.Key, (KeyValuePair<string, int> o) => o.Value);
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00005284 File Offset: 0x00003484
		public static List<Structs.Team> GetTeams()
		{
			List<Structs.Team> list = new List<Structs.Team>();
			using (Dictionary<int, Structs.Team>.Enumerator enumerator = Structs.Values.Teams.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					KeyValuePair<int, Structs.Team> k = enumerator.Current;
					bool flag = Utils.IsPlayer(k.Key) && k.Value != Structs.Team.无 && list.FindAll((Structs.Team t) => t == k.Value).Count == 0;
					if (flag)
					{
						list.Add(k.Value);
					}
				}
			}
			return list;
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00005340 File Offset: 0x00003540
		public static void CountGrades()
		{
			Dictionary<string, int> dictionary = Utils.RankPlayers();
			int num = dictionary.Count;
			foreach (KeyValuePair<string, int> keyValuePair in Utils.RankPlayers())
			{
				Color color = Color.White;
				try
				{
					color = Utils.GetTeamColor(Structs.Values.Teams[TSPlayer.FindByNameOrID(keyValuePair.Key)[0].Index]);
				}
				catch
				{
				}
				TShock.Utils.Broadcast(string.Format("第{0}名：{1}，总分：{2}", num, keyValuePair.Key, keyValuePair.Value), color);
				num--;
			}
			TShock.Utils.Broadcast("以下为玩家总分排名", Color.Yellow);
			Dictionary<Structs.Team, int> dictionary2 = Utils.RankTeams();
			num = dictionary2.Count;
			foreach (KeyValuePair<Structs.Team, int> keyValuePair2 in dictionary2)
			{
				Color teamColor = Utils.GetTeamColor(keyValuePair2.Key);
				Dictionary<string, int> dictionary3 = Utils.InTeamRank(keyValuePair2.Key);
				int num2 = dictionary3.Count;
				foreach (KeyValuePair<string, int> keyValuePair3 in dictionary3)
				{
					TShock.Utils.Broadcast(string.Format("{0}.{1}，总分：{2}", num2, keyValuePair3.Key, keyValuePair3.Value), teamColor);
					num2--;
				}
				TShock.Utils.Broadcast(string.Format("第{0}名：{1}队，总分：{2}", num, keyValuePair2.Key, keyValuePair2.Value), teamColor);
				num--;
			}
			foreach (int num3 in Utils.GetPlayers())
			{
				Color teamColor2 = Utils.GetTeamColor(Structs.Values.Teams[num3]);
				TShock.Utils.Broadcast(Utils.GetPlayerName(num3) ?? "", teamColor2);
			}
			TShock.Utils.Broadcast("剩余玩家:", Color.Yellow);
		}

		// Token: 0x0600004C RID: 76 RVA: 0x000055D4 File Offset: 0x000037D4
		public static Color GetTeamColor(Structs.Team team)
		{
			Color color;
			switch (team)
			{
			case Structs.Team.红:
				color = Color.Red;
				break;
			case Structs.Team.绿:
				color = Color.Green;
				break;
			case Structs.Team.蓝:
				color = Color.Blue;
				break;
			case Structs.Team.黄:
				color = Color.Yellow;
				break;
			case Structs.Team.紫:
				color = Color.Purple;
				break;
			default:
				color = Color.White;
				break;
			}
			return color;
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00005638 File Offset: 0x00003838
		public static void GameOver()
		{
			Config config = Config.GetConfig();
			TShock.Utils.Broadcast("游戏结束,正在清理掉落物", Color.Green);
			Commands.HandleCommand(TSPlayer.Server, "/clear item 99999");
			Structs.Values.DestroyCircleLoop.Abort();
			Thread dropItemLoop = Structs.Values.DropItemLoop;
			if (dropItemLoop != null)
			{
				dropItemLoop.Abort();
			}
			Structs.Values.SafeCircleLoop.Abort();
			TShock.Utils.Broadcast("游戏结束,正在返回大厅", Color.Green);
			Structs.Values.Game = Structs.Status.end;
			foreach (KeyValuePair<int, Structs.Item[]> keyValuePair in Structs.Values.VisitorInvs)
			{
				try
				{
					Utils.Teleport(keyValuePair.Key, config.Hall);
					Utils.SetPlayerInv(keyValuePair.Key, keyValuePair.Value);
				}
				catch
				{
				}
			}
			Structs.Values.VisitorInvs = new Dictionary<int, Structs.Item[]>();
			foreach (int num in Utils.GetAll())
			{
				Utils.Teleport(num, config.Hall);
			}
			Utils.CountGrades();
			List<int> players = Utils.GetPlayers();
			foreach (int num2 in players)
			{
				try
				{
					Structs.Values.playing[num2] = false;
					Utils.PlayerHP(num2, Structs.Values.PlayerHP[num2], Structs.Values.PlayerHP[num2]);
					Utils.SetPlayerInv(num2, Structs.Values.PlayerInvs[num2]);
				}
				catch
				{
				}
			}
			Structs.Values.Killed = new List<int>();
			Structs.Values.Falled = new List<int>();
			Structs.Values.Teams = new Dictionary<int, Structs.Team>();
			Structs.Values.PlayerHP = new Dictionary<int, int>();
			Structs.Values.PlayerInvs = new Dictionary<int, Structs.Item[]>();
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00005858 File Offset: 0x00003A58
		public static void Teleport(int who, Structs.Point p)
		{
			TShock.Players[who].Teleport((float)(p.TileX * 16), (float)(p.TileY * 16), 1);
		}

		// Token: 0x0600004F RID: 79 RVA: 0x00005880 File Offset: 0x00003A80
		public static void ClearPlayerBuff(int player)
		{
			TSPlayer tsplayer = TShock.Players[player];
			for (int i = 0; i < 22; i++)
			{
				tsplayer.TPlayer.DelBuff(i);
			}
		}

		// Token: 0x06000050 RID: 80 RVA: 0x000058B8 File Offset: 0x00003AB8
		public static void Initialize()
		{
			try
			{
				Config config = Config.GetConfig();
				TShock.Utils.Broadcast("正在初始化服务器", Color.Yellow);
				Random random = Structs.Values.random;
				TShock.Utils.Broadcast("正在清理掉落物", Color.Yellow);
				Commands.HandleCommand(TSPlayer.Server, "clear item 100000000");
				TShock.Utils.Broadcast("正在初始化玩家", Color.Yellow);
				Structs.Values.Teams = new Dictionary<int, Structs.Team>();
				Structs.Values.PlayerInvs = new Dictionary<int, Structs.Item[]>();
				Structs.Values.VisitorInvs = new Dictionary<int, Structs.Item[]>();
				foreach (int num in Utils.GetPlayers())
				{
					Structs.Values.Teams.Add(num, (Structs.Team)TShock.Players[num].Team);
					Structs.Values.PlayerInvs.Add(num, Utils.GetPlayerBank(num));
					Utils.InvitatizePlayerInv(num);
				}
				Utils.<>c__DisplayClass20_0 CS$<>8__locals1 = new Utils.<>c__DisplayClass20_0();
				int minItems = config.MinItems;
				int maxItems = config.MaxItems;
				CS$<>8__locals1.i = 0;
				ThreadPool.QueueUserWorkItem(new WaitCallback(CS$<>8__locals1.<Initialize>g__ShowProgress|0));
				CS$<>8__locals1.i = 0;
				while (CS$<>8__locals1.i < 8000)
				{
					bool flag = Main.chest[CS$<>8__locals1.i] != null;
					if (flag)
					{
						foreach (Item item2 in Main.chest[CS$<>8__locals1.i].item)
						{
							item2.netID = 0;
						}
						int num2 = Structs.Values.random.Next(minItems, maxItems);
						for (int j = 0; j < num2; j++)
						{
							Structs.Item item3 = Utils.Next(null);
							Main.chest[CS$<>8__locals1.i].item[j].netID = item3.netID;
							Main.chest[CS$<>8__locals1.i].item[j].stack = item3.stack;
							Main.chest[CS$<>8__locals1.i].item[j].prefix = item3.prefix;
						}
					}
					int i2 = CS$<>8__locals1.i;
					CS$<>8__locals1.i = i2 + 1;
				}
				Thread.Sleep(5000);
				TShock.Utils.Broadcast("正在初始化buff", Color.Yellow);
				foreach (int num3 in Utils.GetPlayers())
				{
					Utils.ClearPlayerBuff(num3);
				}
				Thread.Sleep(5000);
				TShock.Utils.Broadcast("正在初始化生命", Color.Yellow);
				Structs.Values.PlayerHP = new Dictionary<int, int>();
				Structs.Values.Grades = new Dictionary<string, int>();
				foreach (int num4 in Utils.GetPlayers())
				{
					Structs.Values.PlayerHP.Add(num4, TShock.Players[num4].TPlayer.statLifeMax);
					Utils.PlayerHP(num4, config.StartLife, config.StartLife);
					Structs.Values.Grades.Add(Utils.GetPlayerName(num4), 0);
				}
				Thread.Sleep(5000);
				TShock.Utils.Broadcast("初始化玩家完成,正在传送至轨道", Color.Yellow);
				foreach (int num5 in Utils.GetPlayers())
				{
					TSPlayer tsplayer = TShock.Players[num5];
					Utils.Teleport(num5, config.Start);
					tsplayer.SetBuff(211, 3600, false);
					tsplayer.TPlayer.velocity.X = (float)Structs.Values.random.Next(-300, -100);
					tsplayer.SendData(13, "", 0, 0f, 0f, 0f, 0);
				}
				TShock.Utils.Broadcast("游戏初始化完成，安全区开始缩小!", Color.Yellow);
				Structs.Values.SafeCircleLoop = new Thread(new ThreadStart(Utils.SafeCircleLoop));
				Structs.Values.SafeCircleLoop.IsBackground = true;
				Structs.Values.SafeCircleLoop.Start();
				bool flag2 = Config.GetConfig().Airdrops.Length != 0;
				if (flag2)
				{
					Structs.Values.DropItemLoop = new Thread(new ThreadStart(Utils.DropItemLoop));
					Structs.Values.DropItemLoop.IsBackground = true;
					Structs.Values.DropItemLoop.Start();
				}
				Structs.Values.DestroyCircleLoop = new Thread(new ThreadStart(Utils.DestroyCircleLoop));
				Structs.Values.DestroyCircleLoop.IsBackground = true;
				Structs.Values.DestroyCircleLoop.Start();
			}
			catch (Exception ex)
			{
				throw new Exception(ex.StackTrace);
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x00005E08 File Offset: 0x00004008
		public static void PlayerHP(int who, int life, int maxLife)
		{
			TSPlayer tsplayer = TShock.Players[who];
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(8);
					binaryWriter.Write(16);
					binaryWriter.Write((byte)who);
					binaryWriter.Write((short)life);
					binaryWriter.Write((short)maxLife);
				}
				tsplayer.SendRawData(memoryStream.ToArray());
			}
		}

		// Token: 0x06000052 RID: 82 RVA: 0x00005EA0 File Offset: 0x000040A0
		public static void DestroyCircleLoop()
		{
			Config config = Config.GetConfig();
			int num = config.Circle.Break / 3;
			Thread.Sleep(config.Circle.Break);
			for (;;)
			{
				Thread.Sleep(Structs.Values.random.Next(num, 2 * num) * 1000);
				int num2 = Structs.Values.random.Next(Structs.Values.SafeMin, Structs.Values.SafeMax);
				int destroySize = config.DestroySize;
				int destroyTileY = config.DestroyTileY;
				int num3 = num2;
				int num4 = num2 + destroySize;
				bool flag = num2 + destroySize > Structs.Values.SafeMax;
				if (flag)
				{
					num4 = Structs.Values.SafeMax;
					num3 = num2 - (num2 + destroySize - Structs.Values.SafeMax);
				}
				Structs.Values.DestroyMax = num4;
				Structs.Values.DestroyMin = num3;
				Structs.Values.IsDestroy = true;
				TShock.Utils.Broadcast("正在进行随机轰炸,范围：" + Utils.GetTileXInfo(num3) + "-" + Utils.GetTileXInfo(num4), Color.Yellow);
				for (int i = 0; i < 30; i++)
				{
					Thread.Sleep(1000);
					Utils.NewProj(240, Structs.Values.random.Next(num3, num4), destroyTileY, 0, 1);
				}
				Structs.Values.IsDestroy = false;
				TShock.Utils.Broadcast("随机轰炸已结束", Color.Yellow);
			}
		}

		// Token: 0x06000053 RID: 83 RVA: 0x00005FEC File Offset: 0x000041EC
		public static void NewProj(int id, int tilex, int tiley, int vx, int vy)
		{
			try
			{
				Projectile.NewProjectile(null, (float)(tilex * 16), (float)(tiley * 16), (float)(vx * 16), (float)(vy * 16), id, 100, 10f, 255, 0f, 0f);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		// Token: 0x06000054 RID: 84 RVA: 0x00006050 File Offset: 0x00004250
		public static void SetGhost(int who, bool ghost)
		{
			TSPlayer tsplayer = TShock.Players[who];
			for (int i = 0; i < 3; i++)
			{
				tsplayer.TPlayer.ghost = ghost;
				tsplayer.SendData(13, "", 0, 0f, 0f, 0f, 0);
			}
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000060A4 File Offset: 0x000042A4
		public static string GetPlayerName(int who)
		{
			return TShock.Players[who].Name;
		}

		// Token: 0x06000056 RID: 86 RVA: 0x000060C4 File Offset: 0x000042C4
		public static void DropItem(Structs.Item item, int tileX, int tileY)
		{
			int num = Item.NewItem(null, tileX * 16, tileY * 16, 0, 0, item.netID, item.stack, true, (int)item.prefix, true, false);
			TSPlayer.All.SendData(21, "", num, 0f, 0f, 0f, 0);
		}

		// Token: 0x06000057 RID: 87 RVA: 0x0000611C File Offset: 0x0000431C
		public static void DropItemLoop()
		{
			Config config = Config.GetConfig();
			int num = config.Circle.Break / 3;
			Thread.Sleep(config.Circle.Break);
			for (;;)
			{
				Thread.Sleep(Structs.Values.random.Next(num, 2 * num) * 1000);
				int num2 = Structs.Values.random.Next(Structs.Values.SafeMin, Structs.Values.SafeMax);
				TShock.Utils.Broadcast("正在生成空投,坐标:" + Utils.GetTileXInfo(num2), Color.Yellow);
				List<Structs.Item> list = Utils.RandomItemDrop();
				foreach (Structs.Item item in list)
				{
					Utils.DropItem(item, num2, config.DestroyTileY);
				}
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x00006204 File Offset: 0x00004404
		public static bool IsPlayerUnSafe(int who)
		{
			return Structs.Values.UnSafePlayerLoops.ToList<KeyValuePair<int, Thread>>().FindAll((KeyValuePair<int, Thread> k) => k.Key == who).Count != 0;
		}

		// Token: 0x06000059 RID: 89 RVA: 0x00006248 File Offset: 0x00004448
		public static void PlayerOutSafeCircleLoop(object o)
		{
			Config config = Config.GetConfig();
			TSPlayer tsplayer = TShock.Players[(int)o];
			int num = 0;
			tsplayer.SendErrorMessage("您已离开安全区范围,安全区范围:" + Utils.GetTileXInfo(Structs.Values.SafeMin) + "-" + Utils.GetTileXInfo(Structs.Values.SafeMax));
			while (Utils.IsPlayer(tsplayer.Index))
			{
				foreach (int num2 in config.UnSafeBuffs)
				{
					tsplayer.SetBuff(num2, 20, false);
				}
				Thread.Sleep(1000);
				num++;
				bool flag = num == 10;
				if (flag)
				{
					num = 0;
					tsplayer.SendErrorMessage("您已离开安全区范围,安全区范围:" + Utils.GetTileXInfo(Structs.Values.SafeMin) + "-" + Utils.GetTileXInfo(Structs.Values.SafeMax));
				}
			}
		}

		// Token: 0x0600005A RID: 90 RVA: 0x00006324 File Offset: 0x00004524
		public static List<Structs.Item> RandomItemDrop()
		{
			Config config = Config.GetConfig();
			int maxDrop = config.MaxDrop;
			Structs.Item item = null;
			int num = 100;
			for (int i = 0; i < maxDrop; i++)
			{
				Structs.Item item2 = Utils.Next_air(Structs.Values.random);
				bool flag = Utils.GetRate_air(item2.netID) < num;
				if (flag)
				{
					num = Utils.GetRate_air(item2.netID);
					item = item2;
				}
			}
			List<Structs.Item> list = new List<Structs.Item>();
			list.Add(item);
			int num2 = Structs.Values.random.Next(0, maxDrop);
			for (int j = 0; j < num2; j++)
			{
				list.Add(Utils.Next_air(Structs.Values.random));
			}
			return list;
		}

		// Token: 0x0600005B RID: 91 RVA: 0x000063E0 File Offset: 0x000045E0
		public static void SafeCircleLoop()
		{
			Config config = Config.GetConfig();
			Structs.Circle circle = config.Circle;
			TShock.Utils.Broadcast(string.Format("安全区将在{0}秒后缩小至{1}%!", circle.Break, circle.Smaller), Color.Red);
			Structs.Values.SafeMax = Main.maxTilesX;
			Structs.Values.SafeMin = 0;
			bool flag = config.MaxTilesX != -1;
			if (flag)
			{
				Structs.Values.SafeMax = Math.Max(config.MaxTilesX, config.MinTilesX);
			}
			bool flag2 = config.MinTilesX != -1;
			if (flag2)
			{
				Structs.Values.SafeMin = Math.Min(config.MaxTilesX, config.MinTilesX);
			}
			Thread.Sleep(circle.Break * 1000);
			int num = circle.Break;
			for (int i = 0; i < circle.Num; i++)
			{
				num *= circle.Smaller;
				num /= 100;
				int num2 = Structs.Values.random.Next(Structs.Values.SafeMin, Structs.Values.SafeMax);
				int num3 = Structs.Values.SafeMax - Structs.Values.SafeMin;
				num3 *= circle.Smaller;
				num3 /= 100;
				int num4 = Math.Min(Structs.Values.SafeMax, num3 + num2);
				int num5 = num2;
				bool flag3 = num3 + num2 > Structs.Values.SafeMax;
				if (flag3)
				{
					num5 = num2 - (num3 + num2 - Structs.Values.SafeMax);
				}
				int num6 = Structs.Values.SafeMax - Structs.Values.SafeMin - num3;
				int num7 = num6 / num;
				TShock.Utils.Broadcast("安全区开始缩小,范围为" + Utils.GetTileXInfo(num5) + "-" + string.Format("{0},缩圈时间:{1}s", Utils.GetTileXInfo(num4), num), Color.Red);
				int num8 = 0;
				bool flag4 = true;
				for (int j = 0; j < num; j++)
				{
					flag4 = !flag4;
					num8++;
					bool flag5 = num8 == 30;
					if (flag5)
					{
						num8 = 0;
						TShock.Utils.Broadcast(string.Format("剩余缩圈时间:{0},当前安全区范围{1}", num - j, Utils.GetTileXInfo(Structs.Values.SafeMin)) + "-" + Utils.GetTileXInfo(Structs.Values.SafeMax), Color.Red);
					}
					bool flag6 = flag4 && Structs.Values.SafeMax > num4;
					if (flag6)
					{
						Structs.Values.SafeMax -= num7;
					}
					else
					{
						bool flag7 = Structs.Values.SafeMin < num5;
						if (flag7)
						{
							Structs.Values.SafeMin -= num7;
						}
					}
					Thread.Sleep(1000);
				}
				Structs.Values.SafeMax = num4;
				Structs.Values.SafeMin = num5;
				TShock.Utils.Broadcast(string.Format("安全区已停止缩小{0}s后开始第{1}/{2}缩圈", circle.Break, i, circle.Num), Color.Red);
				Thread.Sleep(circle.Break * 1000);
			}
			num *= circle.Smaller;
			num /= 100;
			TShock.Utils.Broadcast(string.Concat(new string[]
			{
				"安全区开始缩小，范围为:全图,缩圈时间:",
				num.ToString(),
				"当前安全区范围为",
				Utils.GetTileXInfo(Structs.Values.SafeMin),
				"-",
				Utils.GetTileXInfo(Structs.Values.SafeMax)
			}), Color.Red);
			int num9 = Structs.Values.SafeMax - Structs.Values.SafeMin;
			int num10 = num9 / num;
			int num11 = 0;
			bool flag8 = true;
			int num12 = Structs.Values.random.Next(Structs.Values.SafeMin, Structs.Values.SafeMax);
			for (int k = 0; k < num; k++)
			{
				flag8 = !flag8;
				num11++;
				bool flag9 = num11 == 30;
				if (flag9)
				{
					num11 = 0;
					TShock.Utils.Broadcast(string.Format("剩余缩圈时间:{0},当前安全区范围{1}", num - k, Utils.GetTileXInfo(Structs.Values.SafeMin)) + "-" + Utils.GetTileXInfo(Structs.Values.SafeMax), Color.Red);
				}
				bool flag10 = flag8 && Structs.Values.SafeMax > num12;
				if (flag10)
				{
					Structs.Values.SafeMax -= num10;
				}
				else
				{
					bool flag11 = Structs.Values.SafeMin < num12;
					if (flag11)
					{
						Structs.Values.SafeMin -= num10;
					}
				}
				Thread.Sleep(1000);
			}
			Structs.Values.SafeMax = 0;
			Structs.Values.SafeMin = 0;
			Utils.GameOver();
		}

		// Token: 0x0600005C RID: 92 RVA: 0x00006838 File Offset: 0x00004A38
		public static string GetTileXInfo(int x)
		{
			int num = Main.maxTilesX / 2;
			bool flag = x > num;
			string text;
			if (flag)
			{
				text = (x - num).ToString() + "以东";
			}
			else
			{
				bool flag2 = num > x;
				if (flag2)
				{
					text = (num - x).ToString() + "以西";
				}
				else
				{
					text = "中心";
				}
			}
			return text;
		}

		// Token: 0x0600005D RID: 93 RVA: 0x0000689C File Offset: 0x00004A9C
		public static void DelRan(int id)
		{
			Config config = Config.GetConfig();
			Structs.RandomItem randomItem = config.RandomItems.ToList<Structs.RandomItem>().Find((Structs.RandomItem rand) => rand.Item.netID == id);
			List<Structs.RandomItem> list = config.RandomItems.ToList<Structs.RandomItem>();
			list.Remove(randomItem);
			config.RandomItems = list.ToArray();
		}

		// Token: 0x0600005E RID: 94 RVA: 0x000068FC File Offset: 0x00004AFC
		public static void DelRan_air(int id)
		{
			Config config = Config.GetConfig();
			Structs.RandomItem randomItem = config.Airdrops.ToList<Structs.RandomItem>().Find((Structs.RandomItem rand) => rand.Item.netID == id);
			List<Structs.RandomItem> list = config.Airdrops.ToList<Structs.RandomItem>();
			list.Remove(randomItem);
			config.Airdrops = list.ToArray();
		}

		// Token: 0x0600005F RID: 95 RVA: 0x0000695C File Offset: 0x00004B5C
		public static bool HasRan(int id)
		{
			Config config = Config.GetConfig();
			return config.RandomItems.ToList<Structs.RandomItem>().FindAll((Structs.RandomItem ran) => ran.Item.netID == id).Count != 0;
		}

		// Token: 0x06000060 RID: 96 RVA: 0x000069A8 File Offset: 0x00004BA8
		public static bool HasRan_air(int id)
		{
			Config config = Config.GetConfig();
			return config.Airdrops.ToList<Structs.RandomItem>().FindAll((Structs.RandomItem ran) => ran.Item.netID == id).Count != 0;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x000069F4 File Offset: 0x00004BF4
		public static void AddRan(Structs.RandomItem ran)
		{
			Config config = Config.GetConfig();
			List<Structs.RandomItem> list = config.RandomItems.ToList<Structs.RandomItem>();
			list.Add(ran);
			config.RandomItems = list.ToArray();
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00006A2C File Offset: 0x00004C2C
		public static void AddRan_air(Structs.RandomItem ran)
		{
			Config config = Config.GetConfig();
			List<Structs.RandomItem> list = config.Airdrops.ToList<Structs.RandomItem>();
			list.Add(ran);
			config.Airdrops = list.ToArray();
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00006A64 File Offset: 0x00004C64
		public static void DelItem(int id)
		{
			Config config = Config.GetConfig();
			List<Structs.Item> list = config.StartInv.ToList<Structs.Item>();
			Structs.Item item = config.StartInv.ToList<Structs.Item>().Find((Structs.Item i) => i.netID == id);
			list.Remove(item);
			config.StartInv = list.ToArray();
		}

		// Token: 0x06000064 RID: 100 RVA: 0x00006AC4 File Offset: 0x00004CC4
		public static bool HasItem(int id)
		{
			Config config = Config.GetConfig();
			return config.StartInv.ToList<Structs.Item>().FindAll((Structs.Item i) => i.netID == id).Count != 0;
		}

		// Token: 0x06000065 RID: 101 RVA: 0x00006B10 File Offset: 0x00004D10
		public static void AddItem(Structs.Item item)
		{
			Config config = Config.GetConfig();
			List<Structs.Item> list = config.StartInv.ToList<Structs.Item>();
			list.Add(item);
			config.StartInv = list.ToArray();
		}

		// Token: 0x06000066 RID: 102 RVA: 0x00006B48 File Offset: 0x00004D48
		public static Structs.Point GetPoint(int who)
		{
			return new Structs.Point
			{
				TileY = TShock.Players[who].TileY,
				TileX = TShock.Players[who].TileX
			};
		}

		// Token: 0x06000067 RID: 103 RVA: 0x00006B88 File Offset: 0x00004D88
		public static bool IsEntered(int who)
		{
			return Utils.GetAll().FindAll((int i) => i == who).Count != 0;
		}

		// Token: 0x06000068 RID: 104 RVA: 0x00006BC8 File Offset: 0x00004DC8
		public static List<int> GetAll()
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, bool> keyValuePair in Structs.Values.playing)
			{
				list.Add(keyValuePair.Key);
			}
			return list;
		}

		// Token: 0x06000069 RID: 105 RVA: 0x00006C30 File Offset: 0x00004E30
		public static bool IsPlayer(int who)
		{
			return Utils.GetPlayers().FindAll((int i) => i == who).Count != 0 && Structs.Values.playing[who];
		}

		// Token: 0x0600006A RID: 106 RVA: 0x00006C80 File Offset: 0x00004E80
		public static List<Structs.Team> GetPlayingTeams()
		{
			List<Structs.Team> list = new List<Structs.Team>();
			foreach (int num in Utils.GetPlayers())
			{
				Structs.Team team = Structs.Values.Teams[num];
				bool flag = team != Structs.Team.无 && list.FindAll((Structs.Team t) => t == team).Count == 0;
				if (flag)
				{
					list.Add(team);
				}
			}
			return list;
		}

		// Token: 0x0600006B RID: 107 RVA: 0x00006D30 File Offset: 0x00004F30
		public static List<int> GetPlayers()
		{
			List<int> list = new List<int>();
			foreach (KeyValuePair<int, bool> keyValuePair in Structs.Values.playing)
			{
				bool value = keyValuePair.Value;
				if (value)
				{
					list.Add(keyValuePair.Key);
				}
			}
			return list;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x00006DA8 File Offset: 0x00004FA8
		public static int GetRate(Structs.Item item)
		{
			Config config = Config.GetConfig();
			return config.RandomItems.ToList<Structs.RandomItem>().Find((Structs.RandomItem ite) => ite.Item == item).Rate;
		}

		// Token: 0x0600006D RID: 109 RVA: 0x00006DF0 File Offset: 0x00004FF0
		public static int GetRate_air(int netID)
		{
			Config config = Config.GetConfig();
			return config.Airdrops.ToList<Structs.RandomItem>().Find((Structs.RandomItem ite) => ite.Item.netID == netID).Rate;
		}

		// Token: 0x0600006E RID: 110 RVA: 0x00006E38 File Offset: 0x00005038
		public static Structs.Item Next(Random random = null)
		{
			Structs.RandomItem[] randomItems = Config.GetConfig().RandomItems;
			random = ((random == null) ? Structs.Values.random : random);
			int num = 0;
			foreach (Structs.RandomItem randomItem in randomItems)
			{
				num += randomItem.Rate;
			}
			int num2 = random.Next(0, num + 1);
			int num3 = 0;
			foreach (Structs.RandomItem randomItem2 in randomItems)
			{
				num3 += randomItem2.Rate;
				int num4 = num3 - randomItem2.Rate;
				bool flag = num2 >= num4 && num2 <= num3;
				if (flag)
				{
					Structs.Item item = randomItem2.Item;
					item.stack = random.Next(1, item.stack + 1);
					return item;
				}
			}
			return randomItems[random.Next(0, randomItems.Length)].Item;
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00006F24 File Offset: 0x00005124
		public static Structs.Item Next_air(Random random = null)
		{
			Structs.RandomItem[] airdrops = Config.GetConfig().Airdrops;
			bool flag = airdrops.Length == 0;
			Structs.Item item;
			if (flag)
			{
				item = new Structs.Item();
			}
			else
			{
				random = ((random == null) ? new Random() : random);
				int num = 0;
				foreach (Structs.RandomItem randomItem in airdrops)
				{
					num += randomItem.Rate;
				}
				int num2 = random.Next(0, num);
				int num3 = 0;
				foreach (Structs.RandomItem randomItem2 in airdrops)
				{
					num3 += randomItem2.Rate;
					int num4 = num3 - randomItem2.Rate;
					bool flag2 = num2 >= num4 && num2 <= num3;
					if (flag2)
					{
						Structs.Item item2 = randomItem2.Item;
						item2.stack = random.Next(1, item2.stack);
						return item2;
					}
				}
				int num5 = random.Next(0, airdrops.Length);
				Structs.Item item3 = airdrops[num5].Item;
				item3.stack = random.Next(1, item3.stack + 1);
				item = item3;
			}
			return item;
		}

		// Token: 0x06000070 RID: 112 RVA: 0x00007044 File Offset: 0x00005244
		public static void SpwanPlayer(int who, int TileX, int TileY)
		{
			TShock.Players[who].Spawn(0, null);
			TShock.Players[who].Teleport((float)(TileX * 16), (float)(TileY * 16), 1);
		}

		// Token: 0x06000071 RID: 113 RVA: 0x00007084 File Offset: 0x00005284
		public static Structs.Item[] GetPlayerBank(int index)
		{
			TSPlayer tsplayer = TShock.Players[index];
			List<Structs.Item> list = Structs.Item.Parse(tsplayer.TPlayer.inventory).ToList<Structs.Item>();
			list.AddRange(Structs.Item.Parse(tsplayer.TPlayer.armor));
			list.AddRange(Structs.Item.Parse(tsplayer.TPlayer.dye));
			list.AddRange(Structs.Item.Parse(tsplayer.TPlayer.miscEquips));
			list.AddRange(Structs.Item.Parse(tsplayer.TPlayer.miscDyes));
			list.Add(Structs.Item.Parse(tsplayer.TPlayer.trashItem));
			return list.ToArray();
		}

		// Token: 0x06000072 RID: 114 RVA: 0x00007130 File Offset: 0x00005330
		public static void InvitatizePlayerInv(int who)
		{
			Config config = Config.GetConfig();
			Structs.Item item = new Structs.Item
			{
				netID = 0,
				stack = 0,
				prefix = 0
			};
			TSPlayer tsplayer = TShock.Players[who];
			int num = tsplayer.TPlayer.inventory.Length + tsplayer.TPlayer.armor.Length + tsplayer.TPlayer.dye.Length + tsplayer.TPlayer.miscEquips.Length + tsplayer.TPlayer.miscDyes.Length + 1;
			Structs.Item[] startInv = config.StartInv;
			Utils.SetPlayerInv(who, startInv);
			tsplayer.TPlayer.trashItem.stack = 0;
			for (int i = startInv.Length; i < num; i++)
			{
				Utils.SetPlayerInvSlot(who, i, item);
			}
			Utils.SetPlayerInvSlot(who, 179, item);
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00007204 File Offset: 0x00005404
		public static void SetPlayerInvSlot(int player, int index, Structs.Item item)
		{
			using (MemoryStream memoryStream = new MemoryStream())
			{
				using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
				{
					binaryWriter.Write(11);
					binaryWriter.Write(5);
					binaryWriter.Write((byte)player);
					binaryWriter.Write((short)index);
					binaryWriter.Write((short)item.stack);
					binaryWriter.Write(item.prefix);
					binaryWriter.Write((short)item.netID);
				}
				TShock.Players[player].SendRawData(memoryStream.ToArray());
			}
		}

		// Token: 0x06000074 RID: 116 RVA: 0x000072B8 File Offset: 0x000054B8
		public static void SetPlayerInv(int player, Structs.Item[] items)
		{
			for (int i = 0; i < items.Length; i++)
			{
				Utils.SetPlayerInvSlot(player, i, items[i]);
			}
		}
	}
}
