using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopTower : MonoBehaviour
{
    public Player playerReference; // Reference to the instance of the other class
    public Tower newTower;
    public Button myButton; // Reference to the button in the scene

    public void ChangeGameObject()
    {
        playerReference.setTower(newTower);
    }

    public void Start()
    {
        // Add a listener to the button's onClick event
        myButton.onClick.AddListener(ChangeGameObject);
    }
    }
