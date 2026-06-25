using UnityEngine;

public static class RectTransformUtil
{
    public static void FitToChildren(RectTransform parent)
    {
        if (parent.childCount == 0)
            return;

        Vector3[] corners = new Vector3[4];

        bool initialized = false;
        Bounds bounds = new Bounds();

        foreach (RectTransform child in parent)
        {
            if (!child.gameObject.activeInHierarchy)
                continue;

            child.GetWorldCorners(corners);

            for (int i = 0; i < 4; i++)
            {
                Vector3 localPos = parent.InverseTransformPoint(corners[i]);

                if (!initialized)
                {
                    bounds = new Bounds(localPos, Vector3.zero);
                    initialized = true;
                }
                else
                {
                    bounds.Encapsulate(localPos);
                }
            }
        }

        if (!initialized)
            return;

        parent.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Horizontal,
            bounds.size.x);

        parent.SetSizeWithCurrentAnchors(
            RectTransform.Axis.Vertical,
            bounds.size.y);
    }
}