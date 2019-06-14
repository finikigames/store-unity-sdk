﻿using System.Collections.Generic;
using UnityEngine;

public class ItemsController : MonoBehaviour
{
    [SerializeField]
    private GameObject _itemContainer_Prefab;

    [SerializeField]
    private GameObject _cartContainer_Prefab;
    
    private Dictionary<string, GameObject> _containers = new Dictionary<string, GameObject>();
    
    public void AddItem(Xsolla.StoreItem itemInformation)
    {
        foreach (var group in itemInformation.groups)
        {
            if (!_containers.ContainsKey(group))
            {
                //create container
                GameObject newContainer = Instantiate(_itemContainer_Prefab, transform);
                newContainer.SetActive(false);
                _containers.Add(group, newContainer);
            }
            //add to container
            GameObject groupContainer = _containers[group];
            groupContainer.GetComponent<ItemContainer>().AddItem(itemInformation);
        }
    }

    public void CreateCart()
    {
	    GameObject newContainer = Instantiate(_cartContainer_Prefab, transform);
	    newContainer.SetActive(false);
	    _containers.Add("CART", newContainer);
    }
    
    public void ActivateContainer(string groupId)
    {
        foreach (var container in _containers.Values)
        {
            container.SetActive(false);
        }
        if (_containers.ContainsKey(groupId))
        {
	        if (groupId != "CART")
	        {
		        _containers[groupId].SetActive(true);
	        }
	        else
	        {
		        var storeController = FindObjectOfType<StoreController>();
		        
		        Xsolla.XsollaStore.Instance.GetCartItems(storeController.Cart, items =>
		        {
			        var cartItemContainer = _containers[groupId].GetComponent<CartItemContainer>();
			        
			        cartItemContainer.ClearCartItems();
			        
			        foreach (var item in items.items)
			        {
				        cartItemContainer.AddCartItem(item);
			        }
			        
			        _containers[groupId].SetActive(true);
		        }, error => print(error.ToString()));
	        }
        }
    }
}