using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentInput : UIInput
{
    protected override void OnSelect(bool isSelected)
    {
        if (isSelected)
        {
            if(selection is ContentInput e)
            {
                e.Deselect();
            }
        }
        base.OnSelect(isSelected);
    }
    public void Deselect()
    {
        base.OnSelect(false);
    }
}
