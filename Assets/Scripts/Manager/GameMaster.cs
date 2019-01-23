using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameMaster : NetworkBehaviour
{
    [Header("UI elements")]
    [Space]
    public GameObject winPanel;
    public GameObject losePanel;
    public Text resetText;

    [Header("Components")]
    [Space]
    public Tilemap gamePlayerTilemap;
    public MapManager mapManager;
    public ItemManager itemManager;

    [SyncVar]
    public bool isGameOver = false;

    private BombSpawner localPlayer;

    private void Update()
    {
        if (isGameOver && isServer && Input.GetKeyDown(KeyCode.R))
        {
            if (isServer)
            {
                ResetGame();
            }
        }
    }

    public void GameOver(BombSpawner deadPlayer)
    {
        isGameOver = true;
        if (deadPlayer.currenPlayerNumber != localPlayer.currenPlayerNumber)
            winPanel.SetActive(true);
        else
            losePanel.SetActive(true);

        if (isServer)
        {
            resetText.text = "Click R to replay";
        }
        else
        {
            resetText.text = "Waiting for host replay";
        }
        resetText.gameObject.SetActive(true);
    }

    [Command]
    public void CmdResetUI()
    {
        RpcResetUI();
    }

    [ClientRpc]
    private void RpcResetUI()
    {
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        resetText.gameObject.SetActive(false);
    }

    public void SetLocalPlayer(BombSpawner player)
    {
        localPlayer = player;
    }

    private void ResetGame()
    {
        CmdResetUI();
        itemManager.CmdRemoveAllItem();
        mapManager.InitDestructableOnTheMap();
        mapManager.CmdRebuildTheMap();
        var players = FindObjectsOfType<BombSpawner>();
        for (int i = 0; i < players.Length; i++)
        {
            players[i].CmdResetPlayer();
        }
        isGameOver = false;
    }

}
