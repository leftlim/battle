using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DynamicScrollView : MonoBehaviour
{
    public int noOfItems;

    public GameObject item;

    public GridLayoutGroup gridLayout;

    public RectTransform scrollContent;

    public ScrollRect scrollRect;


    void OnEnable()
    {
        InitializeList();
    }


    private void ClearOldElement()
    {
        for (int i = 0; i < gridLayout.transform.childCount; i++)
        {
            Destroy(gridLayout.transform.GetChild(i).gameObject);
        }
    }


    public void SetContentHeight()
    {
        float scrollContentHeight = (gridLayout.transform.childCount * gridLayout.cellSize.y) + ((gridLayout.transform.childCount - 1) * gridLayout.spacing.y);
        scrollContent.sizeDelta = new Vector2(400, scrollContentHeight);
    }

    private void InitializeList()
    {
        ClearOldElement();
        for (int i = 0; i < noOfItems; i++)
            InitializeNewItem("" + (i + 1));
        SetContentHeight();
    }

    private GameObject InitializeNewItem(string name)
    {
        GameObject newItem = Instantiate(item) as GameObject;
        newItem.name = name;
        newItem.transform.SetParent(gridLayout.transform, false);
        newItem.transform.localPosition = Vector3.right * 40f;
        newItem.transform.localScale = Vector3.one;
        newItem.SetActive(true);
        return newItem;
    }

    private IEnumerator MoveTowardsTarget(float time,float from,float target) {
        float i = 0;
        float rate = 1 / time;
        while(i<1){
            i += rate * Time.deltaTime;
            scrollRect.verticalNormalizedPosition = Mathf.Lerp(from,target,i);
            yield return 0;
        }
    }

    public GameObject AddNewElement()//생성할때 호출
    {
        GameObject newItem = InitializeNewItem("" + (gridLayout.transform.childCount + 1));
        SetContentHeight();
        StartCoroutine(MoveTowardsTarget(0.2f, scrollRect.verticalNormalizedPosition, 0));
        return newItem;
    }
}
