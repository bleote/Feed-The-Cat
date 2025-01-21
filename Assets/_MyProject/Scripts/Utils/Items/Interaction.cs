using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Interaction : Singleton<Interaction>
{
    #region Variables
    [Header("Settings")]
    [SerializeField] LayerMask itemsLayer;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] LayerMask selectionLayer;
    [SerializeField] GameObject selectionCubePrefab;
    [Header("Debugging")]
    public bool created;
    public bool selected;
    public float pickedDistance = 6;
    public Vector3 size;
    public Vector3 position;
    public Vector3 hitPoint; //used in get Picked item method to get its picked point
    public Vector3 hitDirection; //used in get Picked item method to get its picked point
    public Vector3 deltaPickedPos;
    public Vector3 connectionPoint;
    public GameObject pickedItem;
    public List<GameObject> selectedItems;
    public List<Interactable> items;
    public Action<Vector3> OnObjectUnderGround;
    [HideInInspector] public Transform container; // assign empty gameObject as parent to do action on picked things

    Coroutine moveRoutine;
    Coroutine rotateRoutine;
    float selectionBtnSize;
    Vector3 selectionAreaStartPos;
    Transform selectionCube;
    RectTransform selectionArea;
    #endregion Variables

    #region Start Method
    void Start()
    {
        container = new GameObject("InteractionContainer").transform;
        container.SetParent(transform);
    }
    #endregion Start Method

    #region General
    public void CreateItems(GameObject[] objs, bool scalingAnim = true, Action OnItemCreated = null)
    {
        created = true;
        selected = false;
        AddItems(objs);
        SetItems();
        if (scalingAnim)
            ScallingAnimation(() => { if (OnItemCreated != null) { OnItemCreated(); } });
        container.rotation = Maths.GetNearestRotation(Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0));
    }

    public GameObject[] GrabItems()
    {
        if (!(pickedItem = GrabItem()))
            return null;
        GameObject[] objs = IsItemSelected(pickedItem) ? selectedItems.ToArray() : new GameObject[] { pickedItem };
        return objs;
    }

    public void PickItems(GameObject[] itemToPick, bool selected = false)
    {
        if (itemToPick.Length <= 0)
            return;
        created = false;
        AddItems(itemToPick);
        SetItems();
    }

    public void MoveItems(Vector3 position)
    {
        this.position = position;
        for (int i = 0; i < items.Count; i++)
            items[i].obj.transform.position = position + items[i].offsetPos;
    }

    public void RotateItems(float value, float angle, Action OnItemRotated = null)
    {
        for (int i = 0; i < items.Count; i++)
        {
            int index = i;
            Routine.LerpConstant(items[index].obj.transform.eulerAngles.y, Mathf.RoundToInt((items[index].obj.transform.eulerAngles.y + angle) / 90) * 90, 8, (val) => {
                Vector3 tempPosition = position;
                items[index].obj.transform.RotateAround(position + deltaPickedPos, new Vector3(0, value, 0), val - items[index].obj.transform.eulerAngles.y);
                deltaPickedPos = tempPosition + deltaPickedPos - position;
                items[index].SetOffsetPosition(position);
            },
                () => {
                    if (index < 1 && OnItemRotated != null) { OnItemRotated(); }
                }
            );
        }
    }

    public bool DropItems(bool dropToGround, bool dropingAnim = true, Action OnItemDropped = null)
    {
        if (dropToGround)
        {
            Vector3 targetPos = GetPointOnGround(position) + new Vector3(0, size.y / 2f, 0);
            if (dropingAnim)
            {
                if ((position.y - targetPos.y) > .01f)
                {
                    Routine.Lerp(0, 1, 0.4f, (val) => MoveItems(position + (targetPos - position) * val), () => OnDropCompleted(OnItemDropped));
                    return true;
                }
            }
            else
                MoveItems(targetPos);
        }
        OnDropCompleted(OnItemDropped);
        return false;
    }

    void OnDropCompleted(Action OnItemDropped = null)
    {
        items.Clear();
        if (OnItemDropped != null)
            OnItemDropped();
    }
    public void TrashItems(Vector3 trashAnimTargetPos = default)
    {
        container.position = position;
        SetParent(container);
        for (int i = 0; i < items.Count; i++)
        {
            if (IsItemSelected(items[i].obj))
                selectedItems.Remove(items[i].obj);
            items[i].obj.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off; // turning off shadows
        }
        items.Clear();
        if (trashAnimTargetPos != default)
        {
            rotateRoutine = Routine.Scale(container, Vector3.one, Vector3.one * 0.0001f, .15f); // scaling animation
            moveRoutine = Routine.Move(container, container.position, trashAnimTargetPos, 0.2f); // moving animation
            Routine.WaitAndCall(.5f, () =>
            {
                Routine.Stop(moveRoutine);
                Routine.Stop(rotateRoutine);
                Code.ClearChilds(container);
                container.localScale = Vector3.one;
            });
        }
        else
            Code.ClearChilds(container);
    }

    public void SwapItem(GameObject swapItem)
    {
        if (items.Count != 1) //only change sizes when picked 1 item
            return;
        size = MeshEdges.GetEdges(swapItem).Size();
        Vector3 floorPos = GetPointOnGround(items[0].obj.transform.position);
        if (items[0].obj.transform.position.y + deltaPickedPos.y - size.y / 2f < floorPos.y + .1f) // if below the floor then make it above
        {
            swapItem.transform.position = new Vector3(floorPos.x, floorPos.y + size.y / 2f, floorPos.z); // add extent bcz pivot is in center
            deltaPickedPos += items[0].obj.transform.position - swapItem.transform.position;
        }
        else
            swapItem.transform.position = items[0].obj.transform.position;
        swapItem.transform.rotation = items[0].obj.transform.rotation;
        Destroy(items[0].obj.gameObject);
        if (IsItemSelected(items[0].obj))
            selectedItems.Remove(items[0].obj);
        items[0].obj = swapItem;
        size = MeshEdges.GetEdges(ToArray()).Size();
    }

    public void Reset()
    {
        items.Clear();
        selectedItems.Clear();
        if (selectionCube)
            selectionCube.localScale = Vector3.zero;
    }
    #endregion General

    #region Selection

    public void SelectItems(GameObject[] objs, bool clearPrevious = true)
    {
        if (clearPrevious)
            selectedItems.Clear();
        selectedItems.AddRange(objs);
    }

    public void DeselectAll()
    {
        selectedItems.Clear();
    }

    public bool ToggleItemSelection(GameObject obj)
    {
        bool selected;
        if (selected = selectedItems.Contains(obj))
            selectedItems.Remove(obj);
        else
            selectedItems.Add(obj);
        return !selected;
    }
    public bool IsItemSelected(GameObject obj)
    {
        return selectedItems.Contains(obj);
    }

    public void StartSelectionArea(Vector3 startPoint)
    {
        if (!selectionArea)
        {
            selectionArea = new GameObject("SelectionArea").AddComponent<Image>().GetComponent<RectTransform>();
            selectionArea.GetComponent<Image>().color = new Color(1, .7f, 0, .1f);
            selectionArea.GetComponent<Image>().raycastTarget = false;
            selectionArea.SetParent(FindObjectOfType<Canvas>().transform);
            selectionArea.sizeDelta = Vector2.zero;
            selectionArea.localScale = Vector3.one;
        }
        selectionArea.transform.position = selectionAreaStartPos = startPoint;
    }

    public void DrawSelectionArea(Vector3 endPoint)
    {
        selectionArea.sizeDelta = new Vector2(Mathf.Abs ((selectionAreaStartPos + endPoint).x), Mathf.Abs((selectionAreaStartPos + endPoint).y));
    }
    public void EndSelectionArea()
    {
        selectionArea.sizeDelta = Vector2.zero;
    }

    public void ShowSelectionCube(Vector3 min, Vector3 max)
    {
        if (!selectionCube) {
            selectionCube = Instantiate(selectionCubePrefab).transform; //GameObject.CreatePrimitive(PrimitiveType.Cube).transform; //
            selectionBtnSize = selectionCube.localScale.x * selectionCube.GetChild(0).localScale.x;
        }
        MeshEdges edges = new MeshEdges(min, max);
        selectionCube.position = edges.Center();
        selectionCube.localScale = edges.Size();
        if (selectionCube.localScale.x == 0 || selectionCube.localScale.y == 0 || selectionCube.localScale.z == 0)
            selectionCube.localScale = Vector3.zero;
        else
        {
            for (int i = 0; i < selectionCube.childCount; i++)
                selectionCube.GetChild(i).localScale = i < 2 ? new Vector3(selectionBtnSize / selectionCube.localScale.z, selectionBtnSize / selectionCube.localScale.y, 1) : (i < 4 ? new Vector3(selectionBtnSize / selectionCube.localScale.x, selectionBtnSize / selectionCube.localScale.z, 1) : new Vector3(selectionBtnSize / selectionCube.localScale.x, selectionBtnSize / selectionCube.localScale.y, 1));
        }
    }
    #endregion Selection

    #region Input

    public Vector3 InputWorldPosition()
    {
        Ray ray = Inputs.system.ScreenToRay(Inputs.system.InputPosition());
        Vector3 floorPos = GetPointOnGround(position);
        Vector3 newPosition = ray.origin + ray.direction * pickedDistance - deltaPickedPos;
        if (newPosition.y - size.y / 2f < floorPos.y) // if below the floor then make it above
        {
            ray = new Ray(ray.origin - deltaPickedPos - new Vector3(0, size.y / 2f, 0), ray.direction);
            if (OnObjectUnderGround != null && ray.origin.y < floorPos.y)
                OnObjectUnderGround(new Vector3(0, floorPos.y - ray.origin.y + .1f, 0));
            if (Physics.Raycast(ray, out RaycastHit hit, 100, groundLayer))
                newPosition = hit.point + new Vector3(0, size.y / 2f, 0); // add extent bcz pivot is in center
        }
        return newPosition;
    }

    public Vector3 GetPointOnGround(Vector3 position)
    {
        Ray ray = new Ray(position + Vector3.up * 50, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, 100, groundLayer))
            position = hit.point;
        return position;
    }

    public Vector3 OnClickExtrudeSelectionGizmo()
    {
        RaycastHit hit;
        Ray ray = Inputs.system.ScreenToRay(Inputs.system.InputPosition());
        bool gotHit = Physics.Raycast(ray, out hit, 100, selectionLayer);
        if (gotHit)
        {
            if (hit.collider.gameObject.name.Contains("Gizmo"))
            {
                if (int.Parse(hit.collider.gameObject.name.Substring(5, 1)) == 0)
                    return -hit.collider.gameObject.transform.parent.right * selectionCube.localScale.x;
                if (int.Parse(hit.collider.gameObject.name.Substring(5, 1)) == 1)
                    return hit.collider.gameObject.transform.parent.right * selectionCube.localScale.x;
                if (int.Parse(hit.collider.gameObject.name.Substring(5, 1)) == 2)
                    return hit.collider.gameObject.transform.parent.up * selectionCube.localScale.y;
                if (int.Parse(hit.collider.gameObject.name.Substring(5, 1)) == 3)
                    return -hit.collider.gameObject.transform.parent.up * selectionCube.localScale.y;
                if (int.Parse(hit.collider.gameObject.name.Substring(5, 1)) == 4)
                    return hit.collider.gameObject.transform.parent.forward * selectionCube.localScale.z;
                if (int.Parse(hit.collider.gameObject.name.Substring(5, 1)) == 5)
                    return -hit.collider.gameObject.transform.parent.forward * selectionCube.localScale.z;
            }
        }
        return Vector3.zero;
    }

    readonly int[] INDEXES_X = { 0, 1, 0, 1, 2, 0, 2, 1, 2, 3, 0, 3, 1, 3, 2, 3, 4, 0, 4, 1 };
    readonly int[] INDEXES_Y = { 0, 0, 1, 1, 0, 2, 1, 2, 2, 0, 3, 1, 3, 2, 3, 3, 0, 4, 1, 4 };
    readonly Vector2[] DIRs = { new Vector3(1, 1, 0), new Vector3(1, -1, 0), new Vector3(-1, 1, 0), new Vector3(-1, -1, 0) };
    public GameObject GrabItem()
    {
        float MIN_PIXEL_DISTANCE = .01f;
        GameObject itemSelected = null;
        for (int i = 0; i < INDEXES_X.Length; i++)
        {
            float x = MIN_PIXEL_DISTANCE * INDEXES_X[i], y = MIN_PIXEL_DISTANCE * INDEXES_Y[i];
            for (int j = 0; j < DIRs.Length; j++)
                if (itemSelected = OnClick(new Vector3(x * DIRs[j].x, y * DIRs[j].y, 0)))
                    return itemSelected;
        }
        return itemSelected;
    }

    public GameObject OnClick(Vector3 delta)
    {
        RaycastHit hit;
        Ray ray = Inputs.system.ScreenToRay(Inputs.system.InputPosition());
        ray.direction += delta / 10f; //Debug.DrawLine(ray.origin, ray.origin + ray.direction * 100, Color.magenta, 5);
        bool gotHit = Physics.Raycast(ray, out hit, 100, itemsLayer);
        if (gotHit)
        {
            GameObject itemSelected = hit.collider.gameObject;
            hitPoint = hit.point;
            pickedDistance = (itemSelected.transform.position - Camera.main.transform.position).magnitude;
            return itemSelected;
        }
        return null;
    }
    #endregion Input

    #region Private Methods

    void SetParent(Transform trans = null)
    {
        for (int i = 0; i < items.Count; i++)
            items[i].obj.transform.SetParent(trans == null ? items[i].parent : trans);
    }

    void ScallingAnimation(Action OnFinish)
    {
        for (int i = 0; i < items.Count; i++)
        {
            int index = i;
            Routine.Lerp(0, 1, .15f, //scaling animation when item created from scroller
            (val) => {
                if (index < items.Count)
                    Maths.ScaleAround(items[index].obj, position, Vector3.zero + (Vector3.one - Vector3.zero) * val);
            },
                () => { if (index < 1 && OnFinish != null) { OnFinish(); } }
            );
        }
    }

    void SetItems()
    {
        MeshEdges meshEdges = MeshEdges.GetEdges(ToArray());
        size = meshEdges.Size();
        position = meshEdges.Center();
        UpdateOffsetPositions();
        deltaPickedPos = created ? Vector3.zero : hitPoint - position;
        float tempDistance = new VectorIII(size).MaxAxis() * 10 + 2;
        pickedDistance = created || pickedDistance < tempDistance ? tempDistance : pickedDistance;
    }
    void AddItems(GameObject[] itemsToAdd)
    {
        items.Clear();
        for (int i = 0; i < itemsToAdd.Length; i++)
            items.Add(new Interactable(itemsToAdd[i]));
    }
    void UpdateOffsetPositions()
    {
        for (int i = 0; i < items.Count; i++)
            items[i].SetOffsetPosition(position);
    }
    GameObject[] ToArray()
    {
        GameObject[] itemsList = new GameObject[items.Count];
        for (int i = 0; i < itemsList.Length; i++)
            itemsList[i] = items[i].obj;
        return itemsList;
    }

    #endregion Private Methods
}

[Serializable]
public class Interactable
{
    public GameObject obj;
    public Transform parent;
    public Vector3 offsetPos;
    public Interactable(GameObject obj)
    {
        this.obj = obj;
        parent = obj.transform.parent;
    }
    public void SetOffsetPosition(Vector3 position)
    {
        offsetPos = obj.transform.position - position;
    }
}
