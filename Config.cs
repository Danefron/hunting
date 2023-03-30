using System;
using System.IO;
using Newtonsoft.Json;

namespace Hunting
{
	// Token: 0x02000002 RID: 2
	internal class Config
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		// (set) Token: 0x06000002 RID: 2 RVA: 0x00002058 File Offset: 0x00000258
		public int MinTilesX { get; set; } = -1;

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000003 RID: 3 RVA: 0x00002061 File Offset: 0x00000261
		// (set) Token: 0x06000004 RID: 4 RVA: 0x00002069 File Offset: 0x00000269
		public int MaxTilesX { get; set; } = -1;

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000005 RID: 5 RVA: 0x00002072 File Offset: 0x00000272
		// (set) Token: 0x06000006 RID: 6 RVA: 0x0000207A File Offset: 0x0000027A
		public Structs.Point Hall { get; set; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000007 RID: 7 RVA: 0x00002083 File Offset: 0x00000283
		// (set) Token: 0x06000008 RID: 8 RVA: 0x0000208B File Offset: 0x0000028B
		public Structs.Point Start { get; set; }

		// Token: 0x17000005 RID: 5
		// (get) Token: 0x06000009 RID: 9 RVA: 0x00002094 File Offset: 0x00000294
		// (set) Token: 0x0600000A RID: 10 RVA: 0x0000209C File Offset: 0x0000029C
		public bool Enable { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x0600000B RID: 11 RVA: 0x000020A5 File Offset: 0x000002A5
		// (set) Token: 0x0600000C RID: 12 RVA: 0x000020AD File Offset: 0x000002AD
		public int MaxJoinSec { get; set; }

		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600000D RID: 13 RVA: 0x000020B6 File Offset: 0x000002B6
		// (set) Token: 0x0600000E RID: 14 RVA: 0x000020BE File Offset: 0x000002BE
		public int Starters { get; set; }

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000020C7 File Offset: 0x000002C7
		// (set) Token: 0x06000010 RID: 16 RVA: 0x000020CF File Offset: 0x000002CF
		public Structs.Circle Circle { get; set; }

		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000011 RID: 17 RVA: 0x000020D8 File Offset: 0x000002D8
		// (set) Token: 0x06000012 RID: 18 RVA: 0x000020E0 File Offset: 0x000002E0
		public Structs.Item[] StartInv { get; set; }

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000013 RID: 19 RVA: 0x000020E9 File Offset: 0x000002E9
		// (set) Token: 0x06000014 RID: 20 RVA: 0x000020F1 File Offset: 0x000002F1
		public int MaxItems { get; set; }

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000015 RID: 21 RVA: 0x000020FA File Offset: 0x000002FA
		// (set) Token: 0x06000016 RID: 22 RVA: 0x00002102 File Offset: 0x00000302
		public int MinItems { get; set; }

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000017 RID: 23 RVA: 0x0000210B File Offset: 0x0000030B
		// (set) Token: 0x06000018 RID: 24 RVA: 0x00002113 File Offset: 0x00000313
		public int MaxDrop { get; set; }

		// Token: 0x1700000D RID: 13
		// (get) Token: 0x06000019 RID: 25 RVA: 0x0000211C File Offset: 0x0000031C
		// (set) Token: 0x0600001A RID: 26 RVA: 0x00002124 File Offset: 0x00000324
		public int DestroySize { get; set; }

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x0600001B RID: 27 RVA: 0x0000212D File Offset: 0x0000032D
		// (set) Token: 0x0600001C RID: 28 RVA: 0x00002135 File Offset: 0x00000335
		public int DestroyTileY { get; set; }

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x0600001D RID: 29 RVA: 0x0000213E File Offset: 0x0000033E
		// (set) Token: 0x0600001E RID: 30 RVA: 0x00002146 File Offset: 0x00000346
		public int StartLife { get; set; }

		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600001F RID: 31 RVA: 0x0000214F File Offset: 0x0000034F
		// (set) Token: 0x06000020 RID: 32 RVA: 0x00002157 File Offset: 0x00000357
		public int[] UnSafeBuffs { get; set; }

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x06000021 RID: 33 RVA: 0x00002160 File Offset: 0x00000360
		// (set) Token: 0x06000022 RID: 34 RVA: 0x00002168 File Offset: 0x00000368
		public int[] FalledBuffs { get; set; }

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x06000023 RID: 35 RVA: 0x00002171 File Offset: 0x00000371
		// (set) Token: 0x06000024 RID: 36 RVA: 0x00002179 File Offset: 0x00000379
		public Structs.RandomItem[] RandomItems { get; set; }

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x06000025 RID: 37 RVA: 0x00002182 File Offset: 0x00000382
		// (set) Token: 0x06000026 RID: 38 RVA: 0x0000218A File Offset: 0x0000038A
		public Structs.RandomItem[] Airdrops { get; set; }

		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000027 RID: 39 RVA: 0x00002193 File Offset: 0x00000393
		// (set) Token: 0x06000028 RID: 40 RVA: 0x0000219B File Offset: 0x0000039B
		public int JumpTimer { get; set; }

		// Token: 0x06000029 RID: 41 RVA: 0x000021A4 File Offset: 0x000003A4
		public Config(Structs.Point hall, Structs.Point start, bool enable, int starters, Structs.Circle circle, int maxItem, int minItem, int maxDrop, int destroySize, int destroyTileY, int maxJoinSec, int[] unSafeBuffs, int[] falledBuffs, int startLife, Structs.Item[] startInv, Structs.RandomItem[] randomItems, Structs.RandomItem[] airdrops, int jumpTimer)
		{
			this.Hall = hall;
			this.Start = start;
			this.Enable = enable;
			this.Starters = starters;
			this.MaxItems = maxItem;
			this.MinItems = minItem;
			this.MaxDrop = maxDrop;
			this.Circle = circle;
			this.MaxJoinSec = maxJoinSec;
			this.UnSafeBuffs = unSafeBuffs;
			this.FalledBuffs = falledBuffs;
			this.StartLife = startLife;
			this.DestroySize = destroySize;
			this.DestroyTileY = destroyTileY;
			this.StartInv = startInv;
			this.RandomItems = randomItems;
			this.Airdrops = airdrops;
			this.JumpTimer = jumpTimer;
		}

		// Token: 0x0600002A RID: 42 RVA: 0x00002268 File Offset: 0x00000468
		public static Config GetConfig()
		{
			Config config = new Config(null, null, false, 5, new Structs.Circle
			{
				Break = 180,
				Num = 5,
				Smaller = 70
			}, 5, 1, 1, 400, 200, 30, new int[] { 39, 32 }, new int[] { 197, 20 }, 200, new Structs.Item[0], new Structs.RandomItem[0], new Structs.RandomItem[0], 60);
			bool flag = File.Exists("tshock/Hunting.json");
			if (flag)
			{
				using (StreamReader streamReader = new StreamReader("tshock/Hunting.json"))
				{
					config = JsonConvert.DeserializeObject<Config>(streamReader.ReadToEnd());
				}
			}
			else
			{
				config.Save();
			}
			return config;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x00002344 File Offset: 0x00000544
		public void Save()
		{
			using (StreamWriter streamWriter = new StreamWriter("tshock/Hunting.json"))
			{
				streamWriter.WriteLine(JsonConvert.SerializeObject(this, 1));
			}
		}

		// Token: 0x04000001 RID: 1
		private const string path = "tshock/Hunting.json";
	}
}
