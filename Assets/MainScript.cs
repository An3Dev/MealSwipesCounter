using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;


public class MainScript : MonoBehaviour
{
    public MainUI mainUI;
    const string swipeStatesKey = "SwipeStatesKey";
    int currentCount = 0;
    int countLimit = 12;

    SwipeState[] swipeStatesArray = new SwipeState[0];

    private void Awake()
    {
        PlayerPrefs.DeleteKey(swipeStatesKey);
        string jsonData = PlayerPrefs.GetString(swipeStatesKey, "[]");
        //string jsonData = saturdaySampleData;
        swipeStatesArray = JsonConvert.DeserializeObject<SwipeState[]>(jsonData);

        if (swipeStatesArray.Length <= 0)
        {
            //swipeStatesArray = new SwipeState[] { new SwipeState(DateTime.Now.ToBinary(), 0, 12) };
            swipeStatesArray = new SwipeState[] { new SwipeState(DateTime.Now, 0, 12) };

        }

        SwipeState lastSwipeState = swipeStatesArray[swipeStatesArray.Length - 1];
        currentCount = lastSwipeState.GetSwipes();
        countLimit = lastSwipeState.GetSwipesLimit();
        UpdateUI();

        InvokeRepeating(nameof(PeriodicDayCheck), 0.00001f, 2);
    }

    void PeriodicDayCheck()
    {

        // get date for most recent sunday
        // if the last entry is from past this sunday, then don't do anything
        // if the last entry is not from past that sunday, then reset the count
        // this works even if there is not entry 
        //DateTime mostRecentEntryDate = DateTime.FromBinary(swipeStatesArray[swipeStatesArray.Length - 1].GetDateTimeBinary());
        DateTime mostRecentEntryDate = swipeStatesArray[swipeStatesArray.Length - 1].GetDateTime();

        DateTime sundayAfterMostRecentEntryDate = mostRecentEntryDate;


        sundayAfterMostRecentEntryDate = GetNextSunday(mostRecentEntryDate);
        // print("Next sunday: " + sundayAfterMostRecentEntryDate);
        //print(DateTime.Now.CompareTo(sundayAfterMostRecentEntryDate));

        //var dateOffset = DateTime.Compare(DateTime.Now, sundayAfterMostRecentEntryDate);
        var dateOffset = DateTime.Now.Subtract(sundayAfterMostRecentEntryDate).Days;
        //print(dateOffset + " most recent entry: " + mostRecentEntryDate.Subtract(sundayAfterMostRecentEntryDate).Days);
        // if the most recent entry is before the sunday where the count resets, then reset counts
        if (dateOffset > 0 && mostRecentEntryDate.Subtract(sundayAfterMostRecentEntryDate).Days <= 0)
        {
            currentCount = 0;
            AddEntry(currentCount);
            UpdateStorage();
            UpdateUI();
        }
    }
    
    // returns today's date if today is a sunday, otherwise, returns the next sunday
    DateTime GetNextSunday(DateTime currentDate)
    {
        DateTime nextSunday = currentDate;
        if (currentDate.DayOfWeek == DayOfWeek.Sunday)
            return currentDate;

        for (int i = 0; i < 7; i++)
        {
            DateTime newDay = nextSunday.AddDays(1 * i + 1);
            if (newDay.DayOfWeek.Equals(DayOfWeek.Sunday))
            {
                nextSunday = newDay;
                break;
            }
        }
        
        return nextSunday;
    }

    public void OnClickPlus()
    {
        if (currentCount >= countLimit)
            return;

        currentCount++;
        AddEntry(currentCount);
        UpdateStorage();
        UpdateUI();
    }

    public void OnClickMinus()
    {
        if (currentCount <= 0)
            return;

        currentCount--;
        RemoveEntry(currentCount);
        UpdateStorage();
        UpdateUI();
    }

    void AddEntry(int currentCount)
    {
        DateTime dateTime = DateTime.Now;
        SwipeState[] newArray = new SwipeState[swipeStatesArray.Length + 1];
        for(int i = 0; i < newArray.Length - 1; i++)
        {
            newArray[i] = swipeStatesArray[i];
        }

        //SwipeState newState = new SwipeState(DateTime.Now.ToBinary(), currentCount, countLimit);
        SwipeState newState = new SwipeState(DateTime.Now, currentCount, countLimit);

        newArray[newArray.Length - 1] = newState;
        swipeStatesArray = newArray;
    }
    
    // Removes the last entry
    void RemoveEntry(int currentCount)
    {
        SwipeState[] newArray = new SwipeState[swipeStatesArray.Length - 1];
        for (int i = 0; i < newArray.Length; i++)
        {
            newArray[i] = swipeStatesArray[i];
        }
        swipeStatesArray = newArray;

    }

    void UpdateStorage()
    {
        string jsonString = JsonConvert.SerializeObject(swipeStatesArray);
        print(jsonString);

        PlayerPrefs.SetString(swipeStatesKey, jsonString);
    }

    void UpdateUI()
    {
        mainUI.UpdateUI(currentCount, countLimit);
    }
}

//[Serializable]
//public class SwipeState
//{
//    public long dateTime;
//    public int swipes;
//    public int swipesLimit;

//    public SwipeState(long dateTime, int swipes, int swipesLimit)
//    {
//        this.dateTime = dateTime;
//        this.swipes = swipes;
//        this.swipesLimit = swipesLimit;
//    }

//    public long GetDateTimeBinary()
//    {
//        return dateTime;
//    }

//    public int GetSwipes()
//    {
//        return swipes;
//    }

//    public int GetSwipesLimit()
//    {
//        return swipesLimit;
//    }
//}

[Serializable]
public class SwipeState
{
    public DateTime dateTime;
    public int swipes;
    public int swipesLimit;

    public SwipeState(DateTime dateTime, int swipes, int swipesLimit)
    {
        this.dateTime = dateTime;
        this.swipes = swipes;
        this.swipesLimit = swipesLimit;
    }

    public DateTime GetDateTime()
    {
        return dateTime;
    }

    public int GetSwipes()
    {
        return swipes;
    }

    public int GetSwipesLimit()
    {
        return swipesLimit;
    }
}
