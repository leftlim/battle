using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ScrollItem : MonoBehaviour
{
    public Text indexText;

    public DynamicScrollView dynamicScrollView;

    public void OnRemoveMe()//지울때 호출
    {
        DestroyImmediate(gameObject);
        dynamicScrollView.SetContentHeight();
    }
}
