using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;


/// <summary>
/// This is a class containing generic functions
/// <remarks></remarks>
/// </summary>
public static class Code {

    public static bool IsInternetAvailable()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    public static string Encrypt(string token)
    {
        try
        {
            string textToEncrypt = token;
            string ToReturn = "";
            string publickey = "12345678";
            string secretkey = "87654321";
            byte[] secretkeyByte = { };
            secretkeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
            byte[] publickeybyte = { };
            publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
            MemoryStream ms = null;
            CryptoStream cs = null;
            byte[] inputbyteArray = System.Text.Encoding.UTF8.GetBytes(textToEncrypt);
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                ms = new MemoryStream();
                cs = new CryptoStream(ms, des.CreateEncryptor(publickeybyte, secretkeyByte), CryptoStreamMode.Write);
                cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                cs.FlushFinalBlock();
                ToReturn = Convert.ToBase64String(ms.ToArray());
            }
            return ToReturn;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex.InnerException);
        }
    }
    public static string Decrypt(string token)
    {
        try
        {
            string textToDecrypt = token;
            string ToReturn = "";
            string publickey = "12345678";
            string secretkey = "87654321";
            byte[] privatekeyByte = { };
            privatekeyByte = System.Text.Encoding.UTF8.GetBytes(secretkey);
            byte[] publickeybyte = { };
            publickeybyte = System.Text.Encoding.UTF8.GetBytes(publickey);
            MemoryStream ms = null;
            CryptoStream cs = null;
            byte[] inputbyteArray = new byte[textToDecrypt.Replace(" ", "+").Length];
            inputbyteArray = Convert.FromBase64String(textToDecrypt.Replace(" ", "+"));
            using (DESCryptoServiceProvider des = new DESCryptoServiceProvider())
            {
                ms = new MemoryStream();
                cs = new CryptoStream(ms, des.CreateDecryptor(publickeybyte, privatekeyByte), CryptoStreamMode.Write);
                cs.Write(inputbyteArray, 0, inputbyteArray.Length);
                cs.FlushFinalBlock();
                Encoding encoding = Encoding.UTF8;
                ToReturn = encoding.GetString(ms.ToArray());
            }
            return ToReturn;
        }
        catch (Exception ae)
        {
            throw new Exception(ae.Message, ae.InnerException);
        }
    }
    public static string GenerateRandomString()
    {
        string randomStr = "";
        string st = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        for (int i = 0; i < 10; i++)
            randomStr += st[UnityEngine.Random.Range(0, st.Length)];
        return randomStr;
    }

    private static HashSet<int> _identifierSet = new HashSet<int>();
    public static int GetUniqueIdentifier()
    {
        while (true)
        {
            int identifier = new System.Random().Next(Int32.MaxValue);
            if (_identifierSet.Contains(identifier)) continue;
                _identifierSet.Add(identifier);
            return identifier;
        }
    }

    static AsyncOperation async = null; // When assigned, load is in progress.
    public static IEnumerator LoadASyncLevel(int levelNo, Image logo = null)
    {
        yield return new WaitForSeconds(1.5f);
        async = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(levelNo);
        while (async.progress < 1)
        {
            if (logo)
                logo.color = new Color(255, 255, 255, async.progress);
            yield return new WaitForSeconds(Time.deltaTime);
        }
    }

    public static GameObject Instantiate(GameObject prefab, Transform parent)
    {
        Transform trans = GameObject.Instantiate(prefab).transform;
        Vector3 position = trans.GetComponent<RectTransform>().anchoredPosition;
        Vector3 localScale = trans.localScale;
        trans.SetParent(parent);
        trans.GetComponent<RectTransform>().anchoredPosition = position;
        trans.localScale = localScale;
        return trans.gameObject;
    }

    public static void ClearChilds(Transform content, int i = 0)
    {
        for (; i < content.childCount; i++)
            GameObject.Destroy(content.GetChild(i).gameObject);
    }

    public static Transform AddPanel(GameObject panelPrefab, RectTransform content, float startGap = 0, float gap = 0, float endGap = 0, bool vertical = true, bool fixSize = false)
    {
        RectTransform panel = GameObject.Instantiate(panelPrefab).GetComponent<RectTransform>();
        panel.SetParent(content);
        panel.name = panel.GetSiblingIndex().ToString();
        int index = content.childCount;
        float panelSize = vertical ? panel.sizeDelta.y : panel.sizeDelta.x;
        content.sizeDelta = new Vector2(!vertical ? index * (panelSize + gap) + startGap + endGap : content.sizeDelta.x, vertical ? index * (panelSize + gap) + startGap + endGap : content.sizeDelta.y);
        panel.anchoredPosition = ((panelSize + gap) * (index - 1) + startGap + (!vertical ? panel.pivot.x : 1 - panel.pivot.y) * panelSize) * (vertical ? new Vector2(0, -1) : new Vector2(1, 0));
        panel.localPosition = new Vector3(panel.localPosition.x, panel.localPosition.y, 0); //for VR Worldspace
        panel.localRotation = Quaternion.identity;  //for VR Worldspace
        panel.localScale = Vector3.one;
        if (!fixSize)
            panel.sizeDelta = panelSize * (vertical ? new Vector2(0, 1) : new Vector2(1, 0));
        return panel;
    }

    public static void ShiftPanel(RectTransform content, Transform parent, bool vertical, bool next, int minPanels, int totalPanels, float panelSize, float startGap, float endGap, float gap, Action<RectTransform, int> action = null)
    { //dir true = shift right
        if (parent.childCount < minPanels)
            return;
        int panelNo = int.Parse(parent.GetChild(parent.childCount - 1).name) + 1; //plus panelSize for adding extents of start and bottom
        if (content.childCount > 0 && ((!next && panelNo >= minPanels + 1) || (next && panelNo > (minPanels - 1) && panelNo < totalPanels)))
        {
            RectTransform childToMove = parent.GetChild(next ? 0 : minPanels - 1).GetComponent<RectTransform>();
            float sizeDelta = (panelNo + (next ? 1 : -1)) * (panelSize + gap) + startGap + endGap;
            content.sizeDelta = vertical ? new Vector2(content.sizeDelta.x, sizeDelta) : new Vector2(sizeDelta, content.sizeDelta.y); //startPos = 135, endPos = -55
            childToMove.anchoredPosition = (vertical ? Vector2.down : Vector2.right) * ((panelSize + gap) * (next ? panelNo : panelNo - (minPanels + 1)) + startGap + (!vertical ? childToMove.pivot.x : 1 - childToMove.pivot.y) * panelSize);
            childToMove.gameObject.SetActive(true); // if hided by load category sub panel
            childToMove.SetSiblingIndex(next ? minPanels - 1 : 0); //set new child no
            childToMove.name = (next ? panelNo : panelNo - (minPanels + 1)).ToString();
            action(childToMove, next ? panelNo : panelNo - (minPanels + 1)); //load panel
        }
    }
}
