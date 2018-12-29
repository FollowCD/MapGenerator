
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(Image))]
public class MapBuildingView : MonoBehaviour {

    Button btn;
    Image buildingImg;
    Image taskImg;
    Text taskInfo;
    GameObject taskParent;
    Material chosenMat;

    public void Init(UnityAction onBtnClick)
    {

        btn = GetComponent<Button>();
        btn.transition = Selectable.Transition.None;
        buildingImg = GetComponent<Image>();
        taskParent = transform.Find("task").gameObject;
        chosenMat = Resources.Load<Material>("ChosenMateiral");
        if (!taskParent)
        {
            Debug.LogError("找不到任务parent");
            return;
        }

        var taskIcon = taskParent.transform.Find("taskIcon");
        var taskText = taskParent.transform.Find("taskText");

        if (taskIcon)
        {
            taskImg = taskIcon.GetComponent<Image>();
        }
        else { Debug.LogError("找不到任务Icon"); }

        if (taskText)
        {
            taskInfo = taskText.GetComponent<Text>();
        }
        else { Debug.LogError("找不到任务text"); }

        ActivateTaskImage(false);

        btn.onClick.AddListener(onBtnClick);
    }

    #region 设置transform
    public void SetPosition(Vector2 pos)
    {
        transform.localPosition = pos;
    }

    public void SetParent(Transform parent)
    {
        transform.SetParent(parent);
    }

    public void SetScale(Vector2 scale)
    {
        transform.localScale = new Vector3(scale.x, scale.y, 1);
    }

    public void SetRotation(Vector3 euler)
    {
        transform.eulerAngles = euler;
    }

    public void SetTransform(Transform parent, Vector2 pos, Vector2 scale, Vector3 euler)
    {
        SetParent(parent);
        SetPosition(pos);
        SetScale(scale);
        SetRotation(euler);
    }
    #endregion

    #region 设置显示

    public void SetInteractable(bool interactable)
    {
        btn.interactable = interactable;
    }
    public void ChangeBuildingImage(Sprite sp)
    {
        if (sp != null && buildingImg != null)
        {
            buildingImg.sprite = sp;
            buildingImg.SetNativeSize();
        }
    }

    public void ShowTaskImage(Sprite sp, string content = "")
    {
        if (sp != null && !string.IsNullOrEmpty(content) && taskImg != null && taskInfo != null)
        {
            taskImg.sprite = sp;
            taskImg.SetNativeSize();
            taskInfo.text = content;
            ActivateTaskImage(true);
        }
        else
        {
            ActivateTaskImage(false);
        }
    }

    public void ShowChosenState(bool isChosen)
    {
        if (buildingImg!=null)
        {
            var material = isChosen ? chosenMat : null;
            buildingImg.material = material;
        }
    }

    void ActivateTaskImage(bool show)
    {
        if (taskParent && taskParent.activeSelf!=show)
        {
            taskParent.SetActive(show);
        }
    }
    #endregion
}
