using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// This component is attached to a UI button which when pressed returns a number between 1 and 4 (inclusive)
// This number is assigned to the roll var, which the player object will then retrieve for use in their turn
public class Dice : MonoBehaviour
{
    [SerializeField] private int MIN_DICE_VALUE = 1;
    [SerializeField] private int MAX_DICE_VALUE = 4;
    private Text text;
    public int roll;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponentInChildren<Text>();
    }

    public void RollButton()
    {
        roll = Random.Range(MIN_DICE_VALUE, MAX_DICE_VALUE);
        text.text = roll.ToString();
    }
}
