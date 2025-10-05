using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public LoadingCanvas loadingCanvas = new LoadingCanvas();
    public void Start()
    {
        loadingCanvas.LoadScene("Menu");
    }
}
