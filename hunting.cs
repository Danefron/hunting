using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Microsoft.Xna.Framework;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;

namespace Hunting
{
	// Token: 0x02000003 RID: 3
	[ApiVersion(2, 1)]
	public class Hunting : TerrariaPlugin
	{
		// Token: 0x17000015 RID: 21
		// (get) Token: 0x0600002C RID: 44 RVA: 0x0000238C File Offset: 0x0000058C
		public override string Author
		{
			get
			{
				return "Leader";
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x0600002D RID: 45 RVA: 0x00002393 File Offset: 0x00000593
		public override string Description
		{
			get
			{
				return "一起享受狩猎的乐趣吧！";
			}
		}

		// Token: 0x17000017 RID: 23
		// (get) Token: 0x0600002E RID: 46 RVA: 0x0000239A File Offset: 0x0000059A
		public override string Name
		{
			get
			{
				return "Hunting";
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x0600002F RID: 47 RVA: 0x000023A1 File Offset: 0x000005A1
		public override Version Version
		{
			get
			{
				return new Version(1, 4, 2, 1);
			}
		}

		// Token: 0x06000030 RID: 48 RVA: 0x000023AC File Offset: 0x000005AC
		public Hunting(Main game)
			: base(game)
		{
		}

		// Token: 0x06000031 RID: 49 RVA: 0x000023C4 File Offset: 0x000005C4
		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				ServerApi.Hooks.NetGetData.Deregister(this, new HookHandler<GetDataEventArgs>(this.OnNetGetData));
				ServerApi.Hooks.ServerLeave.Deregister(this, new HookHandler<LeaveEventArgs>(this.OnServerLeave));
				ServerApi.Hooks.NetGreetPlayer.Deregister(this, new HookHandler<GreetPlayerEventArgs>(this.OnGreetPlayer));
				foreach (Command command in this.cmds)
				{
					Commands.ChatCommands.Remove(command);
				}
			}
			base.Dispose(disposing);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x0000248C File Offset: 0x0000068C
		public override void Initialize()
		{
			ServerApi.Hooks.NetGetData.Register(this, new HookHandler<GetDataEventArgs>(this.OnNetGetData));
			ServerApi.Hooks.NetGreetPlayer.Register(this, new HookHandler<GreetPlayerEventArgs>(this.OnGreetPlayer));
			ServerApi.Hooks.ServerLeave.Register(this, new HookHandler<LeaveEventArgs>(this.OnServerLeave));
			this.cmds.Add(new Command("hunting.admin", new CommandDelegate(this.setting), new string[] { "set" }));
			this.cmds.Add(new Command("hunting.use", new CommandDelegate(this.join), new string[] { "join" }));
			this.cmds.Add(new Command("hunting.use", new CommandDelegate(this.count), new string[] { "count" }));
			this.cmds.Add(new Command("hunting.admin", new CommandDelegate(this.over), new string[] { "over" }));
			foreach (Command command in this.cmds)
			{
				Commands.ChatCommands.Add(command);
			}
			Config.GetConfig();
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002608 File Offset: 0x00000808
		private void test(CommandArgs args)
		{
			Utils.SetPlayerInvSlot(args.Player.Index, 0, new Structs.Item
			{
				netID = 1,
				stack = 1
			});
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002634 File Offset: 0x00000834
		private void count(CommandArgs args)
		{
			try
			{
				bool flag = Structs.Values.Game == Structs.Status.sleeping;
				if (flag)
				{
					args.Player.SendErrorMessage("游戏未开始，暂无数据");
				}
				Dictionary<string, int> dictionary = Utils.RankPlayers();
				int num = dictionary.Count;
				foreach (KeyValuePair<string, int> keyValuePair in Utils.RankPlayers())
				{
					args.Player.SendInfoMessage(string.Format("第{0}名：{1}，总分：{2}", num, keyValuePair.Key, keyValuePair.Value));
					num--;
				}
				args.Player.SendInfoMessage("以下为玩家总分排名", new object[] { Color.Yellow });
				Dictionary<Structs.Team, int> dictionary2 = Utils.RankTeams();
				num = dictionary2.Count;
				foreach (KeyValuePair<Structs.Team, int> keyValuePair2 in dictionary2)
				{
					Color teamColor = Utils.GetTeamColor(keyValuePair2.Key);
					Dictionary<string, int> dictionary3 = Utils.InTeamRank(keyValuePair2.Key);
					int num2 = dictionary3.Count;
					foreach (KeyValuePair<string, int> keyValuePair3 in dictionary3)
					{
						args.Player.SendInfoMessage(string.Format("{0}.{1}，总分：{2}", num2, keyValuePair3.Key, keyValuePair3.Value));
						num2--;
					}
					args.Player.SendInfoMessage(string.Format("第{0}名：{1}队，总分：{2}", num, keyValuePair2.Key, keyValuePair2.Value));
					num--;
				}
				foreach (int num3 in Utils.GetPlayers())
				{
					args.Player.SendInfoMessage(Utils.GetPlayerName(num3) ?? "");
				}
				args.Player.SendInfoMessage("剩余玩家:", new object[] { Color.Yellow });
			}
			catch
			{
			}
		}

		// Token: 0x06000035 RID: 53 RVA: 0x000028FC File Offset: 0x00000AFC
		private void over(CommandArgs args)
		{
			try
			{
				Utils.GameOver();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}

		// Token: 0x06000036 RID: 54 RVA: 0x00002930 File Offset: 0x00000B30
		private void join(CommandArgs args)
		{
			Config config = Config.GetConfig();
			bool flag = Utils.IsPlayer(args.Player.Index);
			if (flag)
			{
				args.Player.SendErrorMessage("您已加入游戏，不可重复加入");
			}
			else
			{
				bool flag2 = args.Parameters.Count != 0 && args.Parameters[0] == "leave";
				if (flag2)
				{
					bool ghost = args.Player.TPlayer.ghost;
					if (ghost)
					{
						Utils.SetPlayerInv(args.Player.Index, Structs.Values.VisitorInvs[args.Player.Index]);
						Structs.Values.VisitorInvs.Remove(args.Player.Index);
						Utils.Teleport(args.Player.Index, config.Hall);
						args.Player.SendSuccessMessage("您已被传送回大厅");
					}
					else
					{
						args.Player.SendErrorMessage("您不在观战模式");
					}
				}
				else
				{
					bool flag3 = !config.Enable;
					if (flag3)
					{
						args.Player.SendErrorMessage("游戏未启用，请联系管理员启用");
					}
					else
					{
						bool flag4 = Structs.Values.Game == Structs.Status.playing;
						if (flag4)
						{
							bool flag5 = args.Parameters.Count == 0;
							if (flag5)
							{
								foreach (int num in Utils.GetPlayers())
								{
									args.Player.SendInfoMessage(Utils.GetPlayerName(num));
								}
								args.Player.SendInfoMessage("当前剩余玩家");
								args.Player.SendInfoMessage("游戏正在进行,您可输入/join 玩家名，传送至某一玩家观战");
							}
							else
							{
								bool flag6 = TSPlayer.FindByNameOrID(args.Parameters[0]).Count == 0;
								if (flag6)
								{
									args.Player.SendErrorMessage("不存在玩家:" + args.Parameters[0]);
								}
								else
								{
									TSPlayer tsplayer = TSPlayer.FindByNameOrID(args.Parameters[0])[0];
									bool flag7 = Utils.IsPlayer(tsplayer.Index);
									if (flag7)
									{
										args.Player.Teleport(tsplayer.X, tsplayer.Y, 1);
										Structs.Values.VisitorInvs.Add(args.Player.Index, Utils.GetPlayerBank(args.Player.Index));
										Utils.SetGhost(args.Player.Index, true);
										args.Player.SendSuccessMessage("您已传送至玩家:" + tsplayer.Name + "输入/join leave返回大厅");
									}
								}
							}
						}
						else
						{
							Structs.Values.playing[args.Player.Index] = true;
							TShock.Utils.Broadcast(string.Format("玩家{0}已加入，进度{1}/", args.Player.Name, Utils.GetPlayers().Count) + string.Format("{0}", config.Starters), Color.Blue);
							bool flag8 = Utils.GetPlayers().Count >= config.Starters && Structs.Values.JoinThread == null;
							if (flag8)
							{
								Structs.Values.JoinThread = new Thread(new ThreadStart(Utils.JoinLoop));
								Structs.Values.JoinThread.IsBackground = true;
								Structs.Values.JoinThread.Start();
							}
						}
					}
				}
			}
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002CA4 File Offset: 0x00000EA4
		private void OnServerLeave(LeaveEventArgs args)
		{
			try
			{
				Structs.Item[] array = Structs.Values.VisitorInvs[args.Who];
				Utils.SetPlayerInv(args.Who, array);
				Structs.Values.VisitorInvs.Remove(args.Who);
			}
			catch
			{
			}
			try
			{
				bool flag = Utils.IsPlayer(args.Who);
				if (flag)
				{
					Utils.SetPlayerInv(args.Who, Structs.Values.PlayerInvs[args.Who]);
				}
				Structs.Values.playing.Remove(args.Who);
				Structs.Values.LastPosition.Remove(args.Who);
			}
			catch
			{
			}
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002D60 File Offset: 0x00000F60
		private void OnGreetPlayer(GreetPlayerEventArgs args)
		{
			try
			{
				Structs.Values.playing.Add(args.Who, false);
				Structs.Values.LastPosition.Add(args.Who, new Vector2(TShock.Players[args.Who].X, TShock.Players[args.Who].Y));
			}
			catch
			{
			}
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002DD4 File Offset: 0x00000FD4
		private void setting(CommandArgs args)
		{
			bool flag = args.Parameters.Count == 0 || args.Parameters[0] == "help";
			if (flag)
			{
				args.Player.SendInfoMessage("/set hall,设置当前坐标为大厅坐标");
				args.Player.SendInfoMessage("/set reload,重载配置文件");
				args.Player.SendInfoMessage("/set start,设置当前坐标为玩家初始位置");
				args.Player.SendInfoMessage("/set able,启用/禁用小游戏");
				args.Player.SendInfoMessage("/set starters 数量,设置开始玩家数量");
				args.Player.SendInfoMessage("/set items,设置初始物品");
				args.Player.SendInfoMessage("/set ran,设置随机生成物品");
				args.Player.SendInfoMessage("/set air,设置空投随机物品");
				args.Player.SendInfoMessage("/set drop 数量,设置空投最大生成数量");
				args.Player.SendInfoMessage("/set jump 时间(单位：秒),设置自动跳伞时间");
			}
			else
			{
				Config config = Config.GetConfig();
				string text = args.Parameters[0];
				string text2 = text;
				uint num = <PrivateImplementationDetails>.ComputeStringHash(text2);
				if (num <= 2805947405U)
				{
					if (num <= 981021583U)
					{
						if (num != 858933783U)
						{
							if (num == 981021583U)
							{
								if (text2 == "items")
								{
									bool flag2 = args.Parameters.Count == 1;
									if (flag2)
									{
										args.Player.SendInfoMessage("/set items add 物品id [数量] [前缀],添加初始物品");
										args.Player.SendInfoMessage("/set items del 物品id,删除初始物品");
										args.Player.SendInfoMessage("/set items list,列出初始物品");
										return;
									}
									string text3 = args.Parameters[1];
									string text4 = text3;
									if (!(text4 == "list"))
									{
										if (!(text4 == "del"))
										{
											if (text4 == "add")
											{
												int num2 = int.Parse(args.Parameters[2]);
												int num3 = ((args.Parameters.Count >= 4) ? int.Parse(args.Parameters[3]) : 1);
												int num4 = ((args.Parameters.Count >= 5) ? int.Parse(args.Parameters[4]) : 0);
												bool flag3 = num3 <= 0;
												if (flag3)
												{
													num3 = 1;
												}
												Structs.Item item = new Structs.Item
												{
													netID = num2,
													stack = num3,
													prefix = (byte)num4
												};
												Utils.AddItem(item);
												args.Player.SendSuccessMessage("初始物品：" + Lang.GetItemNameValue(num2) + string.Format("[i/s{0}:{1}],id:{2},前缀:", num3, num2, num2) + string.Format("{0}数量:{1}添加成功", TShock.Utils.GetPrefixById(num4), num3));
											}
										}
										else
										{
											int num5 = int.Parse(args.Parameters[2]);
											bool flag4 = !Utils.HasItem(num5);
											if (flag4)
											{
												args.Player.SendErrorMessage(string.Format("不存在物品:[i:{0}]", num5));
												return;
											}
											Utils.DelItem(num5);
											args.Player.SendSuccessMessage(string.Format("成功删除物品:[i:{0}]", num5));
										}
									}
									else
									{
										foreach (Structs.Item item2 in config.StartInv)
										{
											args.Player.SendInfoMessage(Lang.GetItemNameValue(item2.netID) + string.Format("[i/s{0}:{1}],id:{2},前缀:", item2.stack, item2.netID, item2.netID) + string.Format("{0}数量:{1}", TShock.Utils.GetPrefixById((int)item2.prefix), item2.stack));
										}
										args.Player.SendInfoMessage("初始物品列表如下");
									}
								}
							}
						}
						else if (text2 == "air")
						{
							bool flag5 = args.Parameters.Count == 1;
							if (flag5)
							{
								args.Player.SendInfoMessage("/set air add 概率 id [数量] [前缀],添加空投随机生成物品");
								args.Player.SendInfoMessage("/set air del id,删除空投随机生成物品");
								args.Player.SendInfoMessage("/set air list，列出所有空投随机生成物品");
								return;
							}
							string text5 = args.Parameters[1];
							string text6 = text5;
							if (!(text6 == "list"))
							{
								if (!(text6 == "del"))
								{
									if (text6 == "add")
									{
										int num6 = int.Parse(args.Parameters[2]);
										int num7 = int.Parse(args.Parameters[3]);
										int num8 = ((args.Parameters.Count >= 5) ? int.Parse(args.Parameters[4]) : 1);
										int num9 = ((args.Parameters.Count >= 6) ? int.Parse(args.Parameters[5]) : 0);
										bool flag6 = num6 > 0 && num6 < 100;
										if (flag6)
										{
											bool flag7 = num8 <= 0;
											if (flag7)
											{
												args.Player.SendErrorMessage("请输入正确的数值!");
												return;
											}
											Structs.RandomItem randomItem = new Structs.RandomItem
											{
												Item = new Structs.Item
												{
													netID = num7,
													stack = num8,
													prefix = (byte)num9
												},
												Rate = num6
											};
											Utils.AddRan_air(randomItem);
										}
										args.Player.SendSuccessMessage(string.Format("随机物品:{0}[i/s{1}:{2}],", Lang.GetItemNameValue(num7), num8, num7) + string.Format("前缀：{0},生成概率{1}%,id:{2},数量:{3},添加成功", new object[]
										{
											TShock.Utils.GetPrefixById(num9),
											num6,
											num7,
											num8
										}));
									}
								}
								else
								{
									int num10 = int.Parse(args.Parameters[2]);
									bool flag8 = !Utils.HasRan_air(num10);
									if (flag8)
									{
										args.Player.SendErrorMessage(string.Format("不存在物品:[i:{0}]", num10));
										return;
									}
									Utils.DelRan_air(num10);
									args.Player.SendSuccessMessage(string.Format("成功删除物品[i:{0}]", num10));
								}
							}
							else
							{
								foreach (Structs.RandomItem randomItem2 in config.Airdrops)
								{
									Structs.Item item3 = randomItem2.Item;
									args.Player.SendInfoMessage(string.Format("物品:{0}[i/s{1}:{2}],", Lang.GetItemNameValue(item3.netID), item3.stack, item3.netID) + string.Format("前缀：{0},生成概率{1}%,id:{2},数量:{3}", new object[]
									{
										TShock.Utils.GetPrefixById((int)item3.prefix),
										randomItem2.Rate,
										item3.netID,
										item3.stack
									}));
								}
								args.Player.SendInfoMessage("随机生成物品如下");
							}
						}
					}
					else if (num != 1258233334U)
					{
						if (num != 1697318111U)
						{
							if (num == 2805947405U)
							{
								if (text2 == "jump")
								{
									int num11 = int.Parse(args.Parameters[1]);
									config.JumpTimer = num11;
									args.Player.SendSuccessMessage("设置成功，开局" + num11.ToString() + "s后自动跳伞");
								}
							}
						}
						else if (text2 == "start")
						{
							Structs.Point point = Utils.GetPoint(args.Player.Index);
							config.Start = point;
							args.Player.SendSuccessMessage(string.Format("设置初始位置成功,X:{0},Y:{1}", point.TileX, point.TileY));
						}
					}
					else if (text2 == "ran")
					{
						bool flag9 = args.Parameters.Count == 1;
						if (flag9)
						{
							args.Player.SendInfoMessage("/set ran add 概率 id [数量] [前缀],添加随机生成物品");
							args.Player.SendInfoMessage("/set ran del id,删除随机生成物品");
							args.Player.SendInfoMessage("/set ran list，列出所有随机生成物品");
							args.Player.SendInfoMessage("/set ran edit 最小值 最大值，修改随机生成物品数量");
							return;
						}
						string text7 = args.Parameters[1];
						string text8 = text7;
						if (!(text8 == "eidt"))
						{
							if (!(text8 == "list"))
							{
								if (!(text8 == "del"))
								{
									if (text8 == "add")
									{
										int num12 = int.Parse(args.Parameters[2]);
										int num13 = int.Parse(args.Parameters[3]);
										int num14 = ((args.Parameters.Count >= 5) ? int.Parse(args.Parameters[4]) : 1);
										int num15 = ((args.Parameters.Count >= 6) ? int.Parse(args.Parameters[5]) : 0);
										bool flag10 = num12 > 0 && num12 < 100;
										if (flag10)
										{
											bool flag11 = num14 <= 0;
											if (flag11)
											{
												args.Player.SendErrorMessage("请输入正确的数值!");
												return;
											}
											Structs.RandomItem randomItem3 = new Structs.RandomItem
											{
												Item = new Structs.Item
												{
													netID = num13,
													stack = num14,
													prefix = (byte)num15
												},
												Rate = num12
											};
											Utils.AddRan(randomItem3);
										}
										args.Player.SendSuccessMessage(string.Format("随机物品:{0}[i/s{1}:{2}],", Lang.GetItemNameValue(num13), num14, num13) + string.Format("前缀：{0},生成概率{1}%,id:{2},数量:{3},添加成功", new object[]
										{
											TShock.Utils.GetPrefixById(num15),
											num12,
											num13,
											num14
										}));
									}
								}
								else
								{
									int num16 = int.Parse(args.Parameters[2]);
									bool flag12 = !Utils.HasRan(num16);
									if (flag12)
									{
										args.Player.SendErrorMessage(string.Format("不存在物品:[i:{0}]", num16));
										return;
									}
									Utils.DelRan(num16);
									args.Player.SendSuccessMessage(string.Format("成功删除物品[i:{0}]", num16));
								}
							}
							else
							{
								foreach (Structs.RandomItem randomItem4 in config.RandomItems)
								{
									args.Player.SendInfoMessage(string.Format("物品:{0}[i/s{1}:{2}],", Lang.GetItemNameValue(randomItem4.Item.netID), randomItem4.Item.stack, randomItem4.Item.netID) + string.Format("前缀：{0},生成概率{1}%,id:{2},数量:{3}", new object[]
									{
										TShock.Utils.GetPrefixById((int)randomItem4.Item.prefix),
										randomItem4.Rate,
										randomItem4.Item.netID,
										randomItem4.Item.stack
									}));
								}
								args.Player.SendInfoMessage("随机生成物品如下");
							}
						}
						else
						{
							int num17 = int.Parse(args.Parameters[2]);
							int num18 = int.Parse(args.Parameters[3]);
							int num19 = int.Parse(args.Parameters[4]);
							int num20 = Math.Max(num17, num18);
							int num21 = Math.Min(num17, num18);
							bool flag13 = num21 < 0 || num20 > 40 || num19 < 1;
							if (flag13)
							{
								args.Player.SendErrorMessage("请输入正确的数值");
								return;
							}
							config.MaxItems = num20;
							config.MinItems = num21;
							args.Player.SendSuccessMessage(string.Format("随机生成物品数成功!箱子中将会生成{0}-{1}个物品", num21, num20));
						}
					}
				}
				else if (num <= 3502161714U)
				{
					if (num != 2846199180U)
					{
						if (num == 3502161714U)
						{
							if (text2 == "hall")
							{
								Structs.Point point2 = Utils.GetPoint(args.Player.Index);
								config.Hall = point2;
								args.Player.SendSuccessMessage(string.Format("大厅已设置,X:{0},Y:{1}", point2.TileX, point2.TileY));
							}
						}
					}
					else if (text2 == "drop")
					{
						int num22 = int.Parse(args.Parameters[1]);
						bool flag14 = num22 <= 0;
						if (flag14)
						{
							args.Player.SendErrorMessage("请输入正确的数值!");
							return;
						}
						config.MaxDrop = num22;
						args.Player.SendSuccessMessage("设置成功，空投多生成" + num22.ToString() + "个掉落物");
					}
				}
				else if (num != 3710665365U)
				{
					if (num != 3923192197U)
					{
						if (num == 3984383372U)
						{
							if (text2 == "reload")
							{
								config = Config.GetConfig();
								args.Player.SendSuccessMessage("配置文件重载成功");
							}
						}
					}
					else if (text2 == "starters")
					{
						int num23 = int.Parse(args.Parameters[1]);
						bool flag15 = num23 <= 0;
						if (flag15)
						{
							args.Player.SendErrorMessage("您输入的数值不正确");
							return;
						}
						config.Starters = num23;
						args.Player.SendSuccessMessage("开始玩家数量已被设置为:" + num23.ToString());
					}
				}
				else if (text2 == "able")
				{
					config.Enable = !config.Enable;
					args.Player.SendInfoMessage("小游戏已" + (config.Enable ? "启用" : "禁用"));
				}
				config.Save();
			}
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00003C98 File Offset: 0x00001E98
		private void OnNetGetData(GetDataEventArgs args)
		{
			try
			{
				bool flag = args.MsgID == 31;
				if (flag)
				{
					bool flag2 = Structs.Values.Game != Structs.Status.playing;
					if (flag2)
					{
						return;
					}
					args.Handled = !Utils.IsPlayer(args.Msg.whoAmI);
				}
				bool flag3 = args.MsgID == 13;
				if (flag3)
				{
					bool flag4 = Structs.Values.Game != Structs.Status.playing;
					if (flag4)
					{
						return;
					}
					using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						TSPlayer tsplayer = TShock.Players[(int)binaryReader.ReadByte()];
						bool flag5 = !Utils.IsPlayer(tsplayer.Index);
						if (flag5)
						{
							return;
						}
						binaryReader.ReadBytes(5);
						float num = binaryReader.ReadSingle();
						float num2 = binaryReader.ReadSingle();
						Structs.Values.LastPosition[tsplayer.Index] = new Vector2(num, num2);
						bool flag6 = Utils.IsFalled(tsplayer.Index);
						if (flag6)
						{
							tsplayer.TPlayer.sleeping.isSleeping = true;
							tsplayer.SendData(13, "", 0, 0f, 0f, 0f, 0);
							try
							{
								Structs.SavePlayer savePlayer = Structs.Values.SavePlayerLoop[tsplayer.Index];
								bool flag7 = !Utils.CanSave(savePlayer.who, savePlayer.Saver);
								if (flag7)
								{
									savePlayer.SavePlayerLoop.Abort();
									Structs.Values.SavePlayerLoop.Remove(tsplayer.Index);
									tsplayer.SendErrorMessage("救治中断");
									TShock.Players[savePlayer.Saver].SendErrorMessage("救治玩家" + tsplayer.Name + "中断");
								}
							}
							catch
							{
								foreach (int num3 in Utils.GetTeammates(tsplayer.Index))
								{
									bool flag8 = num3 != tsplayer.Index && Utils.CanSave(tsplayer.Index, num3);
									if (flag8)
									{
										List<int> list = new List<int>();
										list.Add(tsplayer.Index);
										list.Add(num3);
										Structs.Values.SavePlayerLoop.Add(tsplayer.Index, new Structs.SavePlayer
										{
											Saver = num3,
											who = tsplayer.Index,
											SavePlayerLoop = new Thread(new ParameterizedThreadStart(Utils.Save))
										});
										Structs.Values.SavePlayerLoop[tsplayer.Index].SavePlayerLoop.IsBackground = true;
										Structs.Values.SavePlayerLoop[tsplayer.Index].SavePlayerLoop.Start(list);
									}
								}
							}
						}
						int num4 = (int)(num / 16f);
						int num5 = (int)(num2 / 16f);
						bool flag9 = num4 <= Structs.Values.SafeMin || num4 >= Structs.Values.SafeMax;
						if (flag9)
						{
							bool flag10 = !Utils.IsPlayerUnSafe(tsplayer.Index);
							if (flag10)
							{
								Thread thread = new Thread(new ParameterizedThreadStart(Utils.PlayerOutSafeCircleLoop));
								thread.IsBackground = true;
								thread.Start(tsplayer.Index);
								Structs.Values.UnSafePlayerLoops.Add(tsplayer.Index, thread);
							}
						}
						else
						{
							bool flag11 = Utils.IsPlayerUnSafe(tsplayer.Index);
							if (flag11)
							{
								Structs.Values.UnSafePlayerLoops[tsplayer.Index].Abort();
								Structs.Values.UnSafePlayerLoops.Remove(tsplayer.Index);
								tsplayer.SendSuccessMessage("您已进入安全区");
							}
						}
					}
				}
				bool flag12 = args.MsgID == 12;
				if (flag12)
				{
					bool flag13 = Structs.Values.Game != Structs.Status.playing;
					if (flag13)
					{
						return;
					}
					args.Handled = Utils.IsEntered(args.Msg.whoAmI);
				}
				bool flag14 = args.MsgID == 117;
				if (flag14)
				{
					args.Handled = Structs.Values.Game != Structs.Status.playing || !Utils.IsPlayer(args.Msg.whoAmI);
					foreach (int num6 in Structs.Values.playing.Keys)
					{
						TSPlayer tsplayer2 = TShock.Players[num6];
						for (int i = 0; i < 22; i++)
						{
							bool flag15 = tsplayer2.TPlayer.buffType[i] == 211;
							if (flag15)
							{
								args.Handled = true;
							}
						}
					}
					using (BinaryReader binaryReader2 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						byte b = binaryReader2.ReadByte();
						bool flag16 = !Utils.IsPlayer((int)b);
						if (flag16)
						{
							args.Handled = true;
						}
						bool flag17 = Utils.IsFalled(args.Msg.whoAmI);
						if (flag17)
						{
							args.Handled = true;
						}
					}
				}
				bool flag18 = args.MsgID == 16;
				if (flag18)
				{
					bool flag19 = Structs.Values.Game != Structs.Status.playing;
					if (flag19)
					{
						return;
					}
					using (BinaryReader binaryReader3 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						int num7 = (int)binaryReader3.ReadByte();
						bool flag20 = binaryReader3.ReadInt16() == 0;
						bool flag21 = flag20;
						if (flag21)
						{
							bool flag22 = Utils.IsFalled(num7);
							if (flag22)
							{
								Utils.PlayerKilled(num7, num7);
							}
							else
							{
								Utils.PlayerFalled(num7, num7);
							}
						}
					}
				}
				bool flag23 = args.MsgID == 118;
				if (flag23)
				{
					bool flag24 = Structs.Values.Game != Structs.Status.playing;
					if (flag24)
					{
						return;
					}
					args.Handled = true;
					bool flag25 = Structs.Values.Game == Structs.Status.playing;
					if (flag25)
					{
						using (BinaryReader binaryReader4 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
						{
							int num8 = (int)binaryReader4.ReadByte();
							binaryReader4.ReadByte();
							int num9 = (int)binaryReader4.ReadByte();
							bool flag26 = !Utils.IsPlayer(num9) || !Utils.IsPlayer(num8);
							if (flag26)
							{
								return;
							}
							num9 = ((num9 == 255) ? num8 : num9);
							bool flag27 = Utils.IsFalled(num8);
							if (flag27)
							{
								Utils.PlayerKilled(num8, num9);
							}
							else
							{
								Utils.PlayerFalled(num8, num9);
							}
						}
					}
				}
				bool flag28 = args.MsgID == 45;
				if (flag28)
				{
					using (BinaryReader binaryReader5 = new BinaryReader(new MemoryStream(args.Msg.readBuffer, args.Index, args.Length)))
					{
						int num10 = (int)binaryReader5.ReadByte();
						int num11 = (int)binaryReader5.ReadByte();
						bool flag29 = Structs.Values.Game == Structs.Status.playing && Utils.IsPlayer(num10);
						if (flag29)
						{
							args.Handled = true;
							Utils.SetTeam(num10, Structs.Values.Teams[num10]);
							TShock.Players[num10].SendErrorMessage("游戏已开始，禁止切换队伍");
						}
					}
				}
				bool flag30 = Structs.Values.Game == Structs.Status.playing;
				if (flag30)
				{
					bool flag31 = Utils.GetPlayers().Count <= 1;
					if (flag31)
					{
						Utils.GameOver();
					}
					bool flag32 = Utils.GetTeams().Count <= 1;
					if (flag32)
					{
						bool flag33 = true;
						foreach (int num12 in Utils.GetPlayers())
						{
							bool flag34 = Structs.Values.Teams[num12] == Structs.Team.无;
							if (flag34)
							{
								flag33 = false;
							}
						}
						bool flag35 = flag33;
						if (flag35)
						{
							Utils.GameOver();
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		// Token: 0x04000016 RID: 22
		private List<Command> cmds = new List<Command>();
	}
}
