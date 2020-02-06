using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public struct Item
{
    public int number;
    public GameObject prefab;
    public TextMesh stat;
    public string name;
    public void refreshStat(int index)
    {
        stat.text = index + ": " +name + ": " + number;
    }
}
public class arm : MonoBehaviour
{
    public static arm Instance;
    // ILOSC  DZIAŁA!?
    // SCIANY
    // DACHY
    // DRZWI
    // OKNA
    const int coordY = 4;
    float speed = 2.5f;
    public  bool busy = false;
    public bool selecting = false;
    public bool holding = false;
    public bool inWorkshop = true;

    public Item[] items;
    GameObject Selected=null;
    int SelectedID = 0;

    string move = null;
    // Start is called before the first frame update
    private void Awake()
    {
        if(Instance==null)
       Instance = this;
    }
    void Start()
    {
        for(int i =0; i<items.Length; i++)
        {
            items[i].refreshStat(i);
        }

    }

    // Update is called once per frame
    // Update is called once per frame
    public void UpdateArm(string move)
    {
        if (!busy)
        {
            busy = true;
            if (!selecting)
            {
                switch (move)
                {
                    case "up":
                        StartCoroutine(Release(GoToWorkshop(),.6f));
                        break;
                    case "down":
                        if (holding && !inWorkshop)
                        StartCoroutine(Release(Drop(), .6f));
                        break;
                    case "left":
                        StartCoroutine(Release(GoTo(new Vector2(Mathf.Max(transform.position.x - 1, -4), coordY)),.6f));
                        inWorkshop = false;
                        break;
                    case "right":
                        float posX = Mathf.Clamp(transform.position.x + 1, -4,4);
                        StartCoroutine(Release(GoTo(new Vector2(posX , coordY)),.6f));
                        inWorkshop = false;
                        break;
                    default:
                        Debug.Log(move);
                        busy = false;
                        break;
                }
            }
            else
            {
                switch (move)
                {
                    case "0":
                    case "door":
                        StartCoroutine(Release(SelectElement(0,items[0].prefab),.6f));
                        break;
                    case "1":
                    case "roof":
                        StartCoroutine(Release(SelectElement(1,items[1].prefab), .6f));
                        break;
                    case "2":
                    case "wall":
                        StartCoroutine(Release(SelectElement(2,items[2].prefab), .6f));
                    break;
                    case "3":
                    case "window":
                        StartCoroutine(Release(SelectElement(3,items[3].prefab), .6f));
                        break;
                    
                    default:
                        Debug.Log(move);
                        busy = false;
                        break;
                }
            }
            
        }


    }
    IEnumerator GoToWorkshop()
    {
        selecting = true;
        yield return StartCoroutine(GoTo(new Vector2(-6, coordY)));
    }
    IEnumerator Drop()
    {
        float desiredHeight = -(items[SelectedID].prefab.transform.localScale.y == 1 ? 1 : items[SelectedID].prefab.transform.localScale.y / 2);
        yield return StartCoroutine(GoTo(new Vector2(this.transform.position.x,desiredHeight)));
        Selected.transform.parent = null;
        Selected = null;
        yield return StartCoroutine(GoTo(new Vector2(this.transform.position.x, coordY)));
        holding = false;
    }

    IEnumerator SelectElement(int index,GameObject Element)
    {
        
        
        yield return StartCoroutine(GoTo(new Vector2(-6, coordY-2)));
        if (Selected)
        {
            Destroy(Selected);
            items[index].number= items[index].number+1;
            items[index].refreshStat(index);
        }
        else
        {
            if (items[index].number == 0)
            {
                yield return StartCoroutine(GoTo(new Vector2(-6, coordY)));
                yield break;
            }
            items[index].number = items[index].number - 1;
            items[index].refreshStat(index);
            float offset = Element.transform.localScale.y == 1 ? 1 : Element.transform.localScale.y/2;
            Selected = Instantiate(Element, new Vector2(this.transform.position.x,this.transform.position.y-offset),Quaternion.identity);
            Selected.transform.parent = this.transform;
        }
        yield return StartCoroutine(GoTo(new Vector2(-6, coordY )));
        holding = true;
        selecting = false;
    }
    IEnumerator Release(IEnumerator coroutine,float timeAfter)
    {
        yield return StartCoroutine(coroutine);
        yield return new WaitForSeconds(timeAfter);
        busy = false;
    }


    IEnumerator GoTo(Vector2 coord)
    {
        
        float step = speed * Time.deltaTime;
        while (transform.position.y != coord.y )
        {
            Debug.Log(coord);

            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, coord.y), step);
            yield return null;

        }
        while (transform.position.y != coord.y || transform.position.x != coord.x) {

            transform.position = Vector2.MoveTowards(transform.position, coord, step);
            yield return null;

        }


          
    }

    /*
    void takeFromWorkshop(int thing)
    {
        if (numberOfItems[ITEM] < 1) return;
        if (!isMoving && build)
        {
            prop_x = workshop.x + 0.5f;
            prop_y = workshop.y + 1.1f;
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, prop_y), step);
            if (transform.position.y == prop_y)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(prop_x, transform.position.y), step);
            }
            if (transform.position.y == prop_y && transform.position.x == prop_x)
            {
                prefab = Instantiate(items[thing], new Vector2(prop_x, workshop.y + 0.5f), Quaternion.identity);
                prefab.transform.parent = gameObject.transform;
                inWorkshop = true;
                holding = true;
                numberOfItems[thing] -= 1;
                build = false;
            }
        }
    }

    void moveToCoords(int x, int y)
    {
        if (!isMoving && inWorkshop && build)
        {
            prop_x = x + 0.5f;
            prop_y = y + 1.1f;
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, prop_y), step);
            if (transform.position.y == prop_y)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(prop_x, transform.position.y), step);
            }

            if (transform.position.y == prop_y && transform.position.x == prop_x)
            {
                Rigidbody2D gameObjectsRigidBody = prefab.AddComponent<Rigidbody2D>();
                gameObjectsRigidBody.mass = mass;
                prefab.transform.parent = null;
                built.Add(prefab);
                holding = false;
                inWorkshop = false;
                build = false;
            }

       
        }

    }

    void putBackToWorkshop()
    {
        if (!isMoving && putback)
        {
            lastItem = built[built.Count - 1];
            prop_x = lastItem.transform.position.x;
            prop_y = lastItem.transform.position.y + 0.5f;
            
            float step = speed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, prop_y), step);
            if (transform.position.y == prop_y)
            {
                transform.position = Vector2.MoveTowards(transform.position, new Vector2(prop_x, transform.position.y), step);
            }
            if (transform.position.y == prop_y && transform.position.x == prop_x)
            {
                lastItem.GetComponent<Rigidbody2D>().isKinematic = true;
                lastItem.transform.parent = gameObject.transform;
                holding = true;
            }
            if (holding)
            {
                float temp_x = workshop.x;
                float temp_y = workshop.y ;
                

                if (transform.position.y == prop_y)
                {
                    transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, temp_y), step);
                    if (transform.position.y == temp_y){
                        transform.position = Vector2.MoveTowards(transform.position, new Vector2(temp_x, transform.position.y), step);
                        if (transform.position.y == temp_y && transform.position.x == temp_x)
                        {
                            switch (lastItem.name)
                            {
                                case "Wall(Clone)":
                                    numberOfItems[0] += 1;
                                    break;
                                case "Roof(Clone)":
                                    numberOfItems[1] += 1;
                                    break;
                                case "Door(Clone)":
                                    numberOfItems[2] += 1;
                                    break;
                                case "Window(Clone)":
                                    numberOfItems[3] += 1;
                                    break;
                            }
                            lastItem.transform.parent = null;
                            built.Remove(lastItem);
                            Destroy(lastItem);
                            lastItem = null;
                            putback = false;
                            holding = false;
                        }
                    
                    }
                   
                }
               

               
            }


}
}
   */
}
