using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class UserData
{
    public static int currentLvl;
    private static string username_Player;
    private static int openedLvls_Player;
    private static int correctAnswers_Player;
    private static int mistakes_Player;

    static public int OpenedLvls_Player
    {
        get
        {
            return openedLvls_Player;
        }
        set
        {
            if (value <= 0)
                return;
            else
                openedLvls_Player = value;
        }
    }

    static public int CorrectAnswers_Player
    {
        get
        {
            return correctAnswers_Player;
        }
        set
        {
            if (value <= 0)
                return;
            else
                correctAnswers_Player = value;
        }
    }

    static public int Mistakes_Player
    {
        get
        {
            return mistakes_Player;
        }
        set
        {
            if (value <= 0)
                return;
            else
                mistakes_Player = value;
        }
    }

    static public string Username_Player
    {
        get
        {
            return username_Player;
        }
        set
        {
            username_Player = value;
        }
    }

    public static void ClearData()
    {
        currentLvl = 0;
        Username_Player = "";
        CorrectAnswers_Player = 0;
        Mistakes_Player = 0;
        OpenedLvls_Player = 1;
    }
}
