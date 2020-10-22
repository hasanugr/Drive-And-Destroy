using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    public GameObject Gold;
    public GameObject Diamond;

    private TextMeshProUGUI GoldText;
    private TextMeshProUGUI DiamondText;
    private int GoldCount;
    private int DiomonCount;

    private void Awake()
    {
        GoldText = Gold.GetComponent<TextMeshProUGUI>();
        DiamondText = Diamond.GetComponent<TextMeshProUGUI>();

        GoldCount = PlayerPrefs.GetInt("Gold", 0);
        DiomonCount = PlayerPrefs.GetInt("Diamond", 0);
    }

    private void OnEnable()
    {
        ShowGoldAndDiomond();
    }

    private void ShowGoldAndDiomond()
    {
        GoldText.text = GoldCount.ToString();
        DiamondText.text = DiomonCount.ToString();
    }

    public void BuyDiamond(int value)
    {
        DiomonCount += value;
        PlayerPrefs.SetInt("Diamond", DiomonCount);
        ShowGoldAndDiomond();
    }
}
