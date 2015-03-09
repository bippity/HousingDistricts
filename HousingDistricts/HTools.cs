﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using TShockAPI;
using Terraria;

namespace HousingDistricts
{
	class HTools
	{
		internal static string HConfigPath { get { return Path.Combine(TShock.SavePath, "hconfig.json"); } }

		public static void SetupConfig()
		{
			try
			{
				if (File.Exists(HConfigPath))
				{
					HousingDistricts.HConfig = HConfigFile.Read(HConfigPath);
					/* Add all the missing config properties in the json file */
				}
				HousingDistricts.HConfig.Write(HConfigPath);
			}
			catch (Exception ex)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine("Error in config file");
				Console.ForegroundColor = ConsoleColor.Gray;
				TShock.Log.Error("Config Exception");
				TShock.Log.Error(ex.ToString());
			}
		}

		public static void BroadcastToHouse(House house, string text, string playername)
		{
			var I = HousingDistricts.HPlayers.Count;
			for (int i = 0; i < I; i++)
			{
				var player = HousingDistricts.HPlayers[i];
				if (house.HouseArea.Intersects(new Rectangle(player.TSPlayer.TileX, player.TSPlayer.TileY, 1, 1)) && !HouseTools.WorldMismatch(house))
				{
					player.TSPlayer.SendMessage("<House> <" + playername + ">: " + text, Color.LightSkyBlue);
				}
			}
		}

		public static string InAreaHouseName(int x, int y)
		{
			var I = HousingDistricts.Houses.Count;
			for (int i = 0; i < I; i++)
			{
				var house = HousingDistricts.Houses[i];
				if (!HouseTools.WorldMismatch(house) &&
					x >= house.HouseArea.Left && x < house.HouseArea.Right &&
					y >= house.HouseArea.Top && y < house.HouseArea.Bottom)
				{
					return house.Name;
				}
			}
			return null;
		}

		public static void BroadcastToHouseOwners(string housename, string text)
		{
			BroadcastToHouseOwners(HouseTools.GetHouseByName(housename), text);
		}

		public static void BroadcastToHouseOwners(House house, string text)
		{
			var I = house.Owners.Count;
			for (int i = 0; i < I; i++)
			{
				var ID = house.Owners[i];
				foreach (var player in HousingDistricts.HPlayers)
				{
					if (player.name == ID && player.HouseNotifications)
					{
						player.TSPlayer.SendMessage(text, Color.LightSeaGreen);
					}
				}
			}
		}

		public static bool OwnsHouse(string UserID, string housename)
		{
			if (String.IsNullOrWhiteSpace(UserID) || UserID == "0" || String.IsNullOrEmpty(housename)) return false;
			House H = HouseTools.GetHouseByName(housename);
			if (H == null) return false;
			return OwnsHouse(UserID, H);
		}

		public static bool OwnsHouse(string UserID, House house)
		{
			bool isAdmin = false;
			try { isAdmin = TShock.Groups.GetGroupByName(TShock.Users.GetUserByID(Convert.ToInt32(UserID)).Group).HasPermission("house.root"); }
			catch {}
			if (!String.IsNullOrEmpty(UserID) && UserID != "0" && house != null)
			{
				try
				{
					if (house.Owners.Contains(UserID) || isAdmin) return true;
					else return false;
				}
				catch (Exception ex)
				{
					TShock.Log.Error(ex.ToString());
					return false;
				}
			}
			return false;
		}

		public static bool CanVisitHouse(string UserID, House house)
		{
			return (!String.IsNullOrEmpty(UserID) && UserID != "0") && (house.Visitors.Contains(UserID) || house.Owners.Contains(UserID) 
				|| house.Groups.Contains(TShock.Groups.GetGroupByName(TShock.Users.GetUserByID(Convert.ToInt32(UserID)).Group).ToString())); 
		}

		public static bool CanGroupVisitHouse(string GroupID, House house)
		{
			return (!String.IsNullOrEmpty(GroupID) && GroupID != "0") && (house.Groups.Contains(TShock.Groups.GetGroupByName(GroupID).Name));
		}

		public static HPlayer GetPlayerByID(int id)
		{
			var I = HousingDistricts.HPlayers.Count;
			for (int i = 0; i < I; i++)
			{
				var player = HousingDistricts.HPlayers[i];
				if (player.Index == id) return player;
			}

			return new HPlayer();
		}

		public static int MaxSize(TSPlayer ply)
		{
			var I = ply.Group.permissions.Count;
			for (int i = 0; i < I; i++)
			{
				var perm = ply.Group.permissions[i];
				Match Match = Regex.Match(perm, "house\\.size\\.(\\d+)");
				if (Match.Success && Match.Value == perm)
				{
					//Console.WriteLine(Convert.ToInt32(Match.Groups[1].Value));
					return Convert.ToInt32(Match.Groups[1].Value);
				}
			}
			return HousingDistricts.HConfig.MaxHouseSize;
		}

		public static int MaxCount(TSPlayer ply)
		{
			var I = ply.Group.permissions.Count;
			for (int i = 0; i < I; i++)
			{
				var perm = ply.Group.permissions[i];
				Match Match = Regex.Match(perm, "house\\.count\\.(\\d+)");
				if (Match.Success && Match.Value == perm)
				{
					return Convert.ToInt32(Match.Groups[1].Value);
				}
			}
			return HousingDistricts.HConfig.MaxHousesByUsername;
		}
	}
}
