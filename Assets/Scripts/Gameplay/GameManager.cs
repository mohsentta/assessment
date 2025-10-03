using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    [SerializeField] private TcpClient tcp;
    [SerializeField] private NetworkStream stream;
    [SerializeField] private Text questionText;
    [SerializeField] private Button[] answerButtons;
    [SerializeField] private Animator animator;
    [SerializeField] private Slider[] progressSliders;
    [SerializeField] private GameObject winnerPanel;
    [SerializeField] private TMP_Text winnerText;
    [SerializeField] private GameObject[] players;

    public void ConnectHandler()
    {
        Connect();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    async void Connect()
    {
        tcp = new TcpClient();
        await tcp.ConnectAsync("127.0.0.1", 7777);
        stream = tcp.GetStream();
        _ = Listen();          // background read thread
        SendReady();           // tell server we’re here
    }

    async Task Listen()
    {
        byte[] header = new byte[1];
        while (true)
        {
            int n = await stream.ReadAsync(header, 0, 1);
            if (n == 0) break;              // server gone
            switch (header[0])
            {
                case 1: await HandleQuestion(); break;
                case 3: await HandleSpeed(); break;
                case 4: await HandleProgress(); break;
                case 5: await HandleGameOver(); break;
            }
        }
        // disconnect UI here
    }


    async Task<string> ReadStringAsync()
    {
        byte[] lenBytes = new byte[2];
        await stream.ReadAsync(lenBytes, 0, 2);
        ushort len = BitConverter.ToUInt16(lenBytes, 0);
        byte[] strBytes = new byte[len];
        await stream.ReadAsync(strBytes, 0, len);
        return Encoding.UTF8.GetString(strBytes);
    }


    async Task HandleQuestion()
    {
        string text = await ReadStringAsync();
        byte correct = (byte)stream.ReadByte();
        byte optCount = (byte)stream.ReadByte();
        string[] opts = new string[optCount];
        for (int i = 0; i < optCount; i++) opts[i] = await ReadStringAsync();

        // populate UI
        questionText.text = text;
        for (int i = 0; i < answerButtons.Length; i++)
        {
            bool active = i < optCount;
            answerButtons[i].gameObject.SetActive(active);
            if (active)
            {
                answerButtons[i].GetComponentInChildren<TMP_Text>().text = opts[i];
                int capture = i;
                answerButtons[i].onClick.AddListener(() => SendAnswer(capture));
            }
        }
        // 10-s countdown slider – your job
    }

    async Task HandleSpeed()
    {
        byte slot = (byte)stream.ReadByte();
        byte[] buf = new byte[8];
        await stream.ReadAsync(buf, 0, 8);
        float mult = BitConverter.ToSingle(buf, 0);
        float dur = BitConverter.ToSingle(buf, 4);
        players[slot].GetComponent<SimpleWaypointFollowerUpdate>().speedMultiplier = mult;


        // apply to local player only if slot == mySlot, or to everyone for visuals
        //animator.SetFloat("Speed", mult);
    }

    async Task HandleProgress()
    {
        byte[] buf = new byte[16];           // 4 floats
        await stream.ReadAsync(buf, 0, 16);
        for (int i = 0; i < 4; i++)
        {
            float d = BitConverter.ToSingle(buf, i * 4);
            progressSliders[i].value = d / 100f;
        }
    }

    async Task HandleGameOver()
    {
        byte winner = (byte)stream.ReadByte();
        winnerPanel.SetActive(true);
        winnerText.text = $"Winner Player {winner}";
    }

    void SendRaw(byte[] payload)
    {
        stream.Write(payload);
    }

    void SendReady()
    {
        // server doesn’t actually need a payload for ready
        SendRaw(new byte[] { 0 });   // optional
    }

    void SendAnswer(int index)
    {
        // clear old listeners first
        foreach (var b in answerButtons) b.onClick.RemoveAllListeners();
        SendRaw(new byte[] { 2, (byte)index });
    }

}
