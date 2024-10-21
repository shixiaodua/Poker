using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardName
{
    static public string[] hsNames = new string[] { "Spade", "Heart", "Club", "Square" };
    static public Dictionary<int, string> cardNames = new Dictionary<int, string>(){
        {1,"One"},{2,"Two"},{3,"Three"},{4,"Four"},{5,"Five"},{6,"Six"},{7,"Seven"},{8,"Eight"},{9,"Nine"},{10,"Ten"},{11,"Jack"},{12,"Queen"},{13,"King"},{99,"SJoker"},{100,"LJoker"}
    };
    //static public string[] cardNames = new string[] { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Jack", "Queen", "King", };
}
