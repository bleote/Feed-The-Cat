using System.Collections.Generic;
using UnityEngine;


//  --- Connection Objects for hints ---  \\
public class Highlighters : Singleton<Highlighters>
{
    [SerializeField] float blinkingAlpha = .5f;
    [SerializeField] float blinkingSpeed = 0.04f;
    [SerializeField] Material highlighterMaterial;
    [SerializeField] Color highlightColor;

    int sign = 1;
    int m_sphereNo;
    int m_capsuleNo;
    float blinkValue;
    List<Transform> m_spheres = new List<Transform>();
    List<Transform> m_capsules = new List<Transform>();

    void Start()
    {
        highlightColor.a = blinkingAlpha;
        blinkingSpeed *= blinkingAlpha;
    }

    void FixedUpdate() => DoBlinking();

    void DoBlinking()
    {
        blinkValue = blinkValue + blinkingSpeed * sign;
        sign = blinkValue > blinkingAlpha ? -sign : (blinkValue < 0 ? -sign : sign);
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).GetComponent<MeshRenderer>().material.color =  new Color(highlightColor.r, highlightColor.g, highlightColor.b, blinkValue);
    }

    public void Show(bool restart = false)
    {
        blinkValue = 0;
        this.enabled = true;
        m_sphereNo = m_capsuleNo = 0;
    }
    public void Hide()
    {
        this.enabled = false;
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
    }
    public void Refresh()
    {
        m_sphereNo = m_capsuleNo = 0;
        for (int i = 0; i < transform.childCount; i++)
            transform.GetChild(i).gameObject.SetActive(false);
    }
    public void AddSphere(Vector3 position, float radius = .085f)
    {
        if (++m_sphereNo > m_spheres.Count)
        {
            Transform hint = CreateShape(PrimitiveType.Sphere, position);
            hint.localScale = Vector3.one * radius;
            m_spheres.Add(hint);
            Destroy(hint.GetComponent<SphereCollider>());
        }
        m_spheres[m_sphereNo - 1].gameObject.SetActive(true);
        m_spheres[m_sphereNo - 1].position = position;
    }
    public void AddCylinder(Vector3 position, Quaternion rotation, float length, float radius = .05f)
    {
        if (++m_capsuleNo > m_capsules.Count)
        {
            Transform hint = CreateShape(PrimitiveType.Cylinder, position);
            m_capsules.Add(hint);
            Destroy(hint.GetComponent<CapsuleCollider>());
        }
        m_capsules[m_capsuleNo - 1].gameObject.SetActive(true);
        m_capsules[m_capsuleNo - 1].position = position;
        m_capsules[m_capsuleNo - 1].rotation = rotation;
        m_capsules[m_capsuleNo - 1].localScale = new Vector3(radius, length / 2f, radius);
    }
    Transform CreateShape(PrimitiveType type, Vector3 position)
    {
        Transform hint = GameObject.CreatePrimitive(type).transform;
        hint.SetParent(transform);
        hint.position = position;
        hint.GetComponent<MeshRenderer>().material = highlighterMaterial;
        return hint;
    }
}
