﻿using System;

namespace Xsolla.Store
{
	[Serializable]
	public class CartItem
	{
		[Serializable]
		public class Group
		{
			public string external_id;
			public string name;
		}
		[Serializable]
		public class VirtualPrice
		{
			public string sku;
			public string name;
			public string type;
			public string description;
			public string image_url;
			public string amount;
			public string amount_without_discount;
			public bool is_default;
		}
		[Serializable]
		public class InventoryOptions
		{
			[Serializable]
			public class ConsumableOption
			{
				public int? usages_count;
			}
			public StoreItem.InventoryOptions.ConsumableOption consumable;
		}

		public string sku;
		public string name;
		public string type;
		public string description;
		public string image_url;
		public string[] attributes;
		public bool is_free;
		public Group[] groups;
		public CartPrice price;
		public VirtualPrice[] virtual_prices;
		public StoreItem.InventoryOptions inventory_options;
		public int quantity;
	}
}