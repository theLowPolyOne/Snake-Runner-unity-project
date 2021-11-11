using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPart : MonoBehaviour
{
    public class Marker
    {
        public Vector3 position;
        public Vector3 rotation;

        public Marker(Vector3 pos, Vector3 rot)
        {
            position = pos;
            rotation = rot;
        }
    }

    public List<Marker> markerList = new List<Marker>();
    private bool isSwallowing = false;

    void FixedUpdate()
    {
        UpdateMarkerList();
    }

    public void UpdateMarkerList()
    {
        markerList.Add(new Marker(transform.position, transform.eulerAngles));
    }

    public void ClearMarkerList()
    {
        markerList.Clear();
        markerList.Add(new Marker(transform.position, transform.eulerAngles));
    }

    public Marker ClosestMarker(float distanceBetween)
    {
        Marker closestMarker = null;
        float minDistance = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Marker marker in markerList)
        {
            float distance = Vector3.Distance(marker.position, currentPosition);
            if (distance < minDistance && distance >= distanceBetween)
            {
                closestMarker = marker;
                minDistance = distance;
            }
        }
        if (closestMarker != null)
        {
            return closestMarker;
        }
        else
        {
            return markerList[0];
        }
    }

    public IEnumerator SnakeSwallow(float speed, float time)
    {
        if (!isSwallowing)
        {
            isSwallowing = true;
            Vector3 a = transform.localScale;
            Vector3 b = transform.localScale * 2;
            float duraion = 0f;
            float rate = (1f / time) * speed;
            while (duraion < 1f)
            {
                duraion += Time.deltaTime * rate;
                transform.localScale = Vector3.Lerp(a, b, duraion);
                yield return null;
            }

            duraion = 0f;
            while (duraion < 1f)
            {
                duraion += Time.deltaTime * rate;
                transform.localScale = Vector3.Lerp(b, a, duraion);
                yield return null;
            }
            isSwallowing = false;
        }
    }
}
