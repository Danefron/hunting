using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Terraria;

namespace Hunting
{
	// Token: 0x02000004 RID: 4
	internal class Structs
	{
		// Token: 0x02000007 RID: 7
		public enum Team
		{
			// Token: 0x04000018 RID: 24
			无,
			// Token: 0x04000019 RID: 25
			红,
			// Token: 0x0400001A RID: 26
			绿,
			// Token: 0x0400001B RID: 27
			蓝,
			// Token: 0x0400001C RID: 28
			黄,
			// Token: 0x0400001D RID: 29
			紫
		}

		// Token: 0x02000008 RID: 8
		public class SavePlayer
		{
			// Token: 0x17000019 RID: 25
			// (get) Token: 0x06000076 RID: 118 RVA: 0x00007322 File Offset: 0x00005522
			// (set) Token: 0x06000077 RID: 119 RVA: 0x0000732A File Offset: 0x0000552A
			public int Saver { get; set; }

			// Token: 0x1700001A RID: 26
			// (get) Token: 0x06000078 RID: 120 RVA: 0x00007333 File Offset: 0x00005533
			// (set) Token: 0x06000079 RID: 121 RVA: 0x0000733B File Offset: 0x0000553B
			public int who { get; set; }

			// Token: 0x1700001B RID: 27
			// (get) Token: 0x0600007A RID: 122 RVA: 0x00007344 File Offset: 0x00005544
			// (set) Token: 0x0600007B RID: 123 RVA: 0x0000734C File Offset: 0x0000554C
			public Thread SavePlayerLoop { get; set; }
		}

		// Token: 0x02000009 RID: 9
		public static class Values
		{
			// Token: 0x04000021 RID: 33
			public static Dictionary<int, Structs.Item[]> VisitorInvs = new Dictionary<int, Structs.Item[]>();

			// Token: 0x04000022 RID: 34
			public static Dictionary<int, Structs.SavePlayer> SavePlayerLoop = new Dictionary<int, Structs.SavePlayer>();

			// Token: 0x04000023 RID: 35
			public static Thread JoinThread = null;

			// Token: 0x04000024 RID: 36
			public static Dictionary<int, Thread> UnSafePlayerLoops = new Dictionary<int, Thread>();

			// Token: 0x04000025 RID: 37
			public static Dictionary<int, int> PlayerHP = new Dictionary<int, int>();

			// Token: 0x04000026 RID: 38
			public static Structs.Status Game = Structs.Status.sleeping;

			// Token: 0x04000027 RID: 39
			public static Random random = new Random();

			// Token: 0x04000028 RID: 40
			public static List<int> Falled = new List<int>();

			// Token: 0x04000029 RID: 41
			public static List<int> Killed = new List<int>();

			// Token: 0x0400002A RID: 42
			public static Dictionary<int, bool> playing = new Dictionary<int, bool>();

			// Token: 0x0400002B RID: 43
			public static Dictionary<int, Structs.Item[]> PlayerInvs = new Dictionary<int, Structs.Item[]>();

			// Token: 0x0400002C RID: 44
			public static int SafeMin;

			// Token: 0x0400002D RID: 45
			public static int SafeMax;

			// Token: 0x0400002E RID: 46
			public static bool IsDestroy = false;

			// Token: 0x0400002F RID: 47
			public static int DestroyMax;

			// Token: 0x04000030 RID: 48
			public static int DestroyMin;

			// Token: 0x04000031 RID: 49
			public static Dictionary<int, Structs.Team> Teams = new Dictionary<int, Structs.Team>();

			// Token: 0x04000032 RID: 50
			public static Dictionary<int, Vector2> LastPosition = new Dictionary<int, Vector2>();

			// Token: 0x04000033 RID: 51
			public static Dictionary<string, int> Grades = new Dictionary<string, int>();

			// Token: 0x04000034 RID: 52
			public static Thread SafeCircleLoop;

			// Token: 0x04000035 RID: 53
			public static Thread DropItemLoop;

			// Token: 0x04000036 RID: 54
			public static Thread DestroyCircleLoop;
		}

		// Token: 0x0200000A RID: 10
		public enum Status
		{
			// Token: 0x04000038 RID: 56
			sleeping,
			// Token: 0x04000039 RID: 57
			playing,
			// Token: 0x0400003A RID: 58
			end
		}

		// Token: 0x0200000B RID: 11
		public class Circle
		{
			// Token: 0x1700001C RID: 28
			// (get) Token: 0x0600007E RID: 126 RVA: 0x000073F7 File Offset: 0x000055F7
			// (set) Token: 0x0600007F RID: 127 RVA: 0x000073FF File Offset: 0x000055FF
			public int Smaller { get; set; }

			// Token: 0x1700001D RID: 29
			// (get) Token: 0x06000080 RID: 128 RVA: 0x00007408 File Offset: 0x00005608
			// (set) Token: 0x06000081 RID: 129 RVA: 0x00007410 File Offset: 0x00005610
			public int Num { get; set; }

			// Token: 0x1700001E RID: 30
			// (get) Token: 0x06000082 RID: 130 RVA: 0x00007419 File Offset: 0x00005619
			// (set) Token: 0x06000083 RID: 131 RVA: 0x00007421 File Offset: 0x00005621
			public int Break { get; set; }
		}

		// Token: 0x0200000C RID: 12
		public class RandomItem
		{
			// Token: 0x1700001F RID: 31
			// (get) Token: 0x06000085 RID: 133 RVA: 0x00007433 File Offset: 0x00005633
			// (set) Token: 0x06000086 RID: 134 RVA: 0x0000743B File Offset: 0x0000563B
			public int Rate { get; set; }

			// Token: 0x17000020 RID: 32
			// (get) Token: 0x06000087 RID: 135 RVA: 0x00007444 File Offset: 0x00005644
			// (set) Token: 0x06000088 RID: 136 RVA: 0x0000744C File Offset: 0x0000564C
			public Structs.Item Item { get; set; }
		}

		// Token: 0x0200000D RID: 13
		public class Point
		{
			// Token: 0x17000021 RID: 33
			// (get) Token: 0x0600008A RID: 138 RVA: 0x0000745E File Offset: 0x0000565E
			// (set) Token: 0x0600008B RID: 139 RVA: 0x00007466 File Offset: 0x00005666
			public int TileX { get; set; }

			// Token: 0x17000022 RID: 34
			// (get) Token: 0x0600008C RID: 140 RVA: 0x0000746F File Offset: 0x0000566F
			// (set) Token: 0x0600008D RID: 141 RVA: 0x00007477 File Offset: 0x00005677
			public int TileY { get; set; }
		}

		// Token: 0x0200000E RID: 14
		public class Item
		{
			// Token: 0x17000023 RID: 35
			// (get) Token: 0x0600008F RID: 143 RVA: 0x00007489 File Offset: 0x00005689
			// (set) Token: 0x06000090 RID: 144 RVA: 0x00007491 File Offset: 0x00005691
			public int netID { get; set; }

			// Token: 0x17000024 RID: 36
			// (get) Token: 0x06000091 RID: 145 RVA: 0x0000749A File Offset: 0x0000569A
			// (set) Token: 0x06000092 RID: 146 RVA: 0x000074A2 File Offset: 0x000056A2
			public int stack { get; set; }

			// Token: 0x17000025 RID: 37
			// (get) Token: 0x06000093 RID: 147 RVA: 0x000074AB File Offset: 0x000056AB
			// (set) Token: 0x06000094 RID: 148 RVA: 0x000074B3 File Offset: 0x000056B3
			public byte prefix { get; set; }

			// Token: 0x06000095 RID: 149 RVA: 0x000074BC File Offset: 0x000056BC
			public static Structs.Item Parse(Terraria.Item i)
			{
				return new Structs.Item
				{
					netID = i.netID,
					stack = i.stack,
					prefix = i.prefix
				};
			}

			// Token: 0x06000096 RID: 150 RVA: 0x000074FC File Offset: 0x000056FC
			public static Structs.Item[] Parse(Terraria.Item[] i)
			{
				List<Structs.Item> list = new List<Structs.Item>();
				foreach (Terraria.Item item in i)
				{
					list.Add(Structs.Item.Parse(item));
				}
				return list.ToArray();
			}
		}
	}
}
