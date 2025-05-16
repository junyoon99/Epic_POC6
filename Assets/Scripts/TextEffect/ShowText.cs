using TMPro;
using UnityEngine;

public class ShowText : MonoBehaviour
{
    TextMeshPro tmp;
    Color color;
    Vector2 StartPos;
    Vector2 GoalPos;
    float moveDistance = 5f;
    float moveTime = 0.5f;
    float elapsedTime = 0f;
    void Start()
    {
        tmp = GetComponent<TextMeshPro>();
        color = tmp.color;
        StartPos = transform.position;
        GoalPos = (Vector2)transform.position + new Vector2(0, moveDistance);
    }
    void Update()
    {
        Move();
    }

    void Move()
    {
        if (elapsedTime < moveTime)
        {
            elapsedTime += Time.deltaTime;
            transform.position = Vector2.Lerp(StartPos, GoalPos, elapsedTime / moveTime);
            color.a = (moveTime - elapsedTime) / moveTime;
            tmp.color = color;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string text)
    {
        tmp = GetComponent<TextMeshPro>();
        color = tmp.color;
        GoalPos = (Vector2)transform.position + new Vector2(0, moveDistance);
        tmp.text = text;
    }

    public void fontSize(float defaultSize, float addsize) 
    {
        TextMeshPro text = GetComponent<TextMeshPro>();
        text.fontSize = defaultSize + addsize;
        text.fontSize = Mathf.Clamp(text.fontSize, 18, 72);
    }
}