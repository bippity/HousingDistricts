﻿using System;
using System.Collections.Generic;
using System.Text;
using TShockAPI.DB;
using TShockAPI;
using Terraria;

namespace HousingDistricts
{
	public class House
	{
		public Rectangle HouseArea { get; set; }
		public List<string> Owners { get; set; }
		public int ID { get; set; }
		public string Name { get; set; }
		public string WorldID { get; set; }
		public int Locked { get; set; }
		public int ChatEnabled { get; set; }
		public List<string> Visitors { get; set; }
		public List<string> Groups { get; set; }

		public House(Rectangle housearea, List<string> owners, int id, string name, string worldid, int locked, int chatenabled, List<string> visitors, List<string> groups)
		{
			HouseArea = housearea;
			Owners = owners;
			ID = id;
			Name = name;
			WorldID = worldid;
			Locked = locked;
			ChatEnabled = chatenabled;
			Visitors = visitors;
			Groups = groups;
		}
	}

	public class HouseTools
	{
		public static bool AddHouse(int tx, int ty, int width, int height, string housename, string owner, int locked, int chatenabled)
		{
			List<string> cols = new List<string>();
			cols.Add("Name");
			cols.Add("TopX");
			cols.Add("TopY");
			cols.Add("BottomX");
			cols.Add("BottomY");
			cols.Add("Owners");
			cols.Add("WorldID");
			cols.Add("Locked");
			cols.Add("ChatEnabled");
			cols.Add("Visitors");
			cols.Add("Groups");
			if (GetHouseByName(housename) != null) { return false; }
			try
			{
				TShock.DB.Query("INSERT INTO HousingDistrict (" + String.Join(", ", cols) + ") VALUES (@0, @1, @2, @3, @4, @5, @6, @7, @8, @9, @10);", housename, tx, ty, width, height, "", Main.worldID.ToString(), locked, chatenabled, "", "");
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
			}

			HousingDistricts.Houses.Add(new House(new Rectangle(tx, ty, width, height), new List<string>(), (HousingDistricts.Houses.Count + 1), housename, Main.worldID.ToString(), locked, chatenabled, new List<string>(), new List<string>()));
			return true;
		}

		public static bool AddNewUser(string houseName, string id)
		{
			var house = GetHouseByName(houseName);
			if (house == null) { return false; }
			StringBuilder sb = new StringBuilder();
			int count = 0;
			house.Owners.Add(id);
			var I = house.Owners.Count;
			for (int i = 0; i < I; i++)
			{
				var owner = house.Owners[i];
				count++;
				sb.Append(owner);
				if(count != house.Owners.Count)
					sb.Append(",");
			}

			try
			{
				string query = "UPDATE HousingDistrict SET Owners=@0 WHERE Name=@1";

				TShock.DB.Query(query, sb.ToString(), houseName);
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				return false;
			}

			return true;
		}

		public static bool DeleteUser(string houseName, string id)
		{
			var house = GetHouseByName(houseName);
			if (house == null) { return false; }
			StringBuilder sb = new StringBuilder();
			int count = 0;
			house.Owners.Remove(id);
			var I = house.Owners.Count;
			for (int i = 0; i < I; i++)
			{
				var owner = house.Owners[i];
				count++;
				sb.Append(owner);
				if (count != house.Owners.Count)
					sb.Append(",");
			}

			try
			{
				string query = "UPDATE HousingDistrict SET Owners=@0 WHERE Name=@1";

				TShock.DB.Query(query, sb.ToString(), houseName);
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				return false;
			}

			return true;
		}

		public static bool WorldMismatch(House house)
		{
			return (house.WorldID != Main.worldID.ToString());
		}

		public static bool AddNewVisitor(House house, string id)
		{
			StringBuilder sb = new StringBuilder();
			int count = 0;
			house.Visitors.Add(id);
			var I = house.Visitors.Count;
			for (int i = 0; i < I; i++)
			{
				var visitor = house.Visitors[i];
				count++;
				sb.Append(visitor);
				if (count != house.Visitors.Count)
					sb.Append(",");
			}

			try
			{
				string query = "UPDATE HousingDistrict SET Visitors=@0 WHERE Name=@1";

				TShock.DB.Query(query, sb.ToString(), house.Name);
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				return false;
			}

			return true;
		}

		public static bool DeleteVisitor(House house, string id)
		{
			StringBuilder sb = new StringBuilder();
			int count = 0;
			house.Visitors.Remove(id);
			var I = house.Visitors.Count;
			for (int i = 0; i < I; i++)
			{
				var visitor = house.Visitors[i];
				count++;
				sb.Append(visitor);
				if (count != house.Visitors.Count)
					sb.Append(",");
			}

			try
			{
				string query = "UPDATE HousingDistrict SET Visitors=@0 WHERE Name=@1";

				TShock.DB.Query(query, sb.ToString(), house.Name);
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				return false;
			}

			return true;
		}

		public static bool AddNewGroup(House house, string id)
		{
			StringBuilder sb = new StringBuilder();
			int count = 0;
			house.Groups.Add(id);
			var I = house.Groups.Count;
			for (int i = 0; i < I; i++)
			{
				var group = house.Groups[i];
				count++;
				sb.Append(group);
				if (count != house.Groups.Count)
					sb.Append(",");
			}

			try
			{
				string query = "UPDATE HousingDistrict SET Groups=@0 WHERE Name=@1";

				TShock.DB.Query(query, sb.ToString(), house.Name);
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				TShock.Log.Error("Exception in AddNewGroup");
				return false;
			}

			return true;
		}

		public static bool DeleteGroup(House house, string id)
		{
			StringBuilder sb = new StringBuilder();
			int count = 0;
			house.Groups.Remove(id);
			var I = house.Groups.Count;
			for (int i = 0; i < I; i++)
			{
				var group = house.Groups[i];
				count++;
				sb.Append(group);
				if (count != house.Groups.Count)
					sb.Append(",");
			}

			try
			{
				string query = "UPDATE HousingDistrict SET Groups=@0 WHERE Name=@1";

				TShock.DB.Query(query, sb.ToString(), house.Name);
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				return false;
			}

			return true;
		}

		public static bool ToggleChat(House house, int onOrOff)
		{
			house.ChatEnabled = onOrOff;

			try
			{
				string query = "UPDATE HousingDistrict SET ChatEnabled=@0 WHERE Name=@1";
				TShock.DB.Query(query, house.ChatEnabled.ToString(), house.Name);
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				return false;
			}

			return true;
		}
		public static bool ChangeLock(House house)
		{
			bool locked = false;

			if (house.Locked == 0)
				locked = true;
			else
				locked = false;

			house.Locked = locked ? 1 : 0;

			try
			{
				string query = "UPDATE HousingDistrict SET Locked=@0 WHERE Name=@1";

				TShock.DB.Query(query, locked ? 1 : 0, house.Name);
			}
			catch (Exception ex)
			{
				TShock.Log.Error(ex.ToString());
				return false;
			}

			return locked;
		}
		public static bool RedefineHouse(int tx, int ty, int width, int height, string housename)
		{
			try
			{
				var house = GetHouseByName(housename);
				var houseName = house.Name;

				try
				{
					string query = "UPDATE HousingDistrict SET TopX=@0, TopY=@1, BottomX=@2, BottomY=@3, WorldID=@4 WHERE Name=@5";

					TShock.DB.Query(query, tx, ty, width, height, Main.worldID.ToString(), house.Name);
				}
				catch (Exception ex)
				{
					TShock.Log.Error(ex.ToString());
					return false;
				}

				house.HouseArea = new Rectangle(tx, ty, width, height);

				return true;
			}
			catch (Exception ex)
			{
				TShock.Log.Error("Error on redefining house: \n" + ex);
				return false;
			}
		}


		public static House GetHouseByName(string name)
		{
			if (String.IsNullOrEmpty(name))
			{
				return null;
			}
			var I = HousingDistricts.Houses.Count;
			for (int i = 0; i < I; i++)
			{
				var house = HousingDistricts.Houses[i];
				if (house.Name == name)
					return house;
			}
			return null;
		}
	}
}
