using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedureMap : MonoBehaviour
{
    public Transform Father = null;
    public GameObject[] Modulars = new GameObject[3];
    public int CeilUnit = 8;
    public Vector2Int Size = new Vector2Int(14, 14);

    private void Start()
    {
        var count = new Vector3Int(0, 0, 0);
        var countR = new Vector4(0, 0, 0);
        var temp = new Vector2Int(-1, -1);
        for (int y = 0; y < this.Size.y; y++)
        {
            for (int x = 0; x < this.Size.x; x++)
            {
                var index = Random.Range(0, 3);
                var direction = Random.Range(0, 4);
                while (true)
                {
                    if (index == temp.x && direction == temp.y)
                    {
                        index = Random.Range(0, 3);
                        direction = Random.Range(0, 4);
                        continue;
                    }
                    else
                    {
                        temp.x = index;
                        temp.y = direction;
                        break;
                    }
                }



                var modular = Instantiate(this.Modulars[index], this.Father);
                modular.transform.position = new Vector3(x * this.CeilUnit + this.CeilUnit, 0, y * this.CeilUnit);


                if (direction == 0) { countR.x++; }
                else if (direction == 1)// 右旋转1次
                {
                    modular.transform.eulerAngles = new Vector3(0f, 90f, 0f);
                    modular.transform.Translate(-this.CeilUnit, 0f, 0f, Space.World);
                    countR.y++;
                }
                else if (direction == 2)// 右旋转2次
                {
                    modular.transform.eulerAngles = new Vector3(0f, 180f, 0f);
                    modular.transform.Translate(-this.CeilUnit, 0, this.CeilUnit, Space.World);
                    countR.z++;
                }
                else if (direction == 3)// 右旋转3次
                {
                    modular.transform.eulerAngles = new Vector3(0f, -90f, 0f);
                    modular.transform.Translate(-0f, 0, this.CeilUnit, Space.World);
                    countR.w++;
                }

                switch (index)
                {
                    case 0: count.x++; break;
                    case 1: count.y++; break;
                    case 2: count.z++; break;
                    default: break;
                }
            }
        }

        Debug.Log(count);
        Debug.Log(countR);
    }

}
