using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Proba;

public class LeaderBoardSample : MonoBehaviour
{
    #region Variables

    public Text leaderBoardText;
    private List<string> leaderBoardIDs = new List<string>();

    #endregion

    #region Register

    void Start()
    {
        if (PROBA.UserHasRegistered())
        {
            PROBA.Initialize();
        }
        else
        {
            PROBA.CheckProgressionStatus();
        }
    }

    private void ProgressionStatusReceived(bool ProgressionStatus)
    {
        if (ProgressionStatus)
        {
            PROBA.Register("Username", false);
        }
        else
        {
            PROBA.Register("Username", true);
        }
    }

    #endregion

    #region Show All LeaderBoards

    public void GetAllLeaderBoards()
    {
        PROBA.GetLeaderBoardsList();
    }

    private void AllLeaderBoardsReceived(IList<LeaderBoardViewModel> leaderBoards)
    {
        if (leaderBoards.Count == 0)
        {
            leaderBoardText.text = "There Is No LeaderBoard";
            return;
        }

        var leaderBoardsName = "";
        foreach (var leaderBoard in leaderBoards)
        {
            leaderBoardsName += leaderBoard.LeaderBoardEnName + "\n";
            leaderBoardIDs.Add(leaderBoard.ID);
        }

        leaderBoardText.text = leaderBoardsName;
    }

    private void AllLeaderBoardsError(RequestResponse response)
    {
        leaderBoardText.text = response.ToString();
    }

    #endregion

    #region Show LeaderBoard Players

    public void GetLeaderBoardUsers()
    {
        if (leaderBoardIDs.Count == 0)
        {
            leaderBoardText.text = "leaderBoard ID List is Empty.\n Add leaderBoard in Dashboard or Click on 'Show all leaderBoards' to Fill List.";
            return;
        }
        PROBA.GetLeaderBoardUsersList(true, leaderBoardIDs[0]);
    }

    private void LeaderBoardUsersReceived(IList<LeaderBoardUserViewModel> leaderBoardUsers)
    {
        if (leaderBoardUsers.Count == 0)
        {
            leaderBoardText.text = "There Is No User In LeaderBoard";
            return;
        }

        var leaderBoardUsersName = "";
        foreach (var leaderBoardUser in leaderBoardUsers)
        {
            leaderBoardUsersName += leaderBoardUser.UserName + "\n";
        }

        leaderBoardText.text = leaderBoardUsersName;
    }

    private void LeaderBoardUsersError(RequestResponse response)
    {
        leaderBoardText.text = response.ToString();
    }

    #endregion

    #region Add Player To LeaderBoard

    public void AddPlayerToLeaderBoard()
    {
        if (leaderBoardIDs.Count == 0)
        {
            leaderBoardText.text = "leaderBoard ID List is Empty.\n Add leaderBoard in Dashboard or Click on 'Show all leaderBoards' to Fill List.";
            return;
        }
        PROBA.UserNewLeaderBoardScore(leaderBoardIDs[0], 10);
    }

    #endregion


    void OnEnable()
    {
        PROBA.OnProgressionStatusReceived += ProgressionStatusReceived;

        PROBA.OnLeaderBoardsListReceived += AllLeaderBoardsReceived;
        PROBA.OnLeaderBoardsListCanceled += AllLeaderBoardsError;

        PROBA.OnLeaderBoardUserListReceived += LeaderBoardUsersReceived;
        PROBA.OnLeaderBoardUserListCanceled += LeaderBoardUsersError;
    }

    void OnDisable()
    {
        PROBA.OnProgressionStatusReceived += ProgressionStatusReceived;

        PROBA.OnLeaderBoardsListReceived -= AllLeaderBoardsReceived;
        PROBA.OnLeaderBoardsListCanceled -= AllLeaderBoardsError;

        PROBA.OnLeaderBoardUserListReceived -= LeaderBoardUsersReceived;
        PROBA.OnLeaderBoardUserListCanceled -= LeaderBoardUsersError;
    }
}
