using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arm : MonoBehaviour
{
    public float speed = 5.0f;
    public float mass = 1000.0f;
    private Vector2 workshop;
    private Vector2 position;
    private float prop_x;
    private float prop_y;
    private bool isMoving = false;
    private bool inWorkshop = false;
    private bool holding = false;
    private GameObject prefab;
    public List<GameObject> built = new List<GameObject>();
    private bool build = false;
    private bool putback = false;
    private GameObject lastItem;
    public GameObject[] items;

    // ILOSC 
    // SCIANY
    // DACHY
    // DRZWI
    // OKNA
    public int[] numberOfItems = new int[] { 10, 1, 1, 3 };
    private GameObject statsWall;
    private GameObject statsRoof;
    private GameObject statsDoor;
    private GameObject statsWindow;
    private int ITEM = 0;
    private int X = 0;
    private int Y = 3;
    // Start is called before the first frame update
    void Start()
    {
        workshop = new Vector2(-6.0f, 3.4f);
        position = gameObject.transform.position;


        statsWall = GameObject.Find("Stats_Wall");
        statsRoof = GameObject.Find("Stats_Roof");
        statsDoor = GameObject.Find("Stats_Door");
        statsWindow = GameObject.Find("Stats_Window");


    }

    // Update is called once per frame
    void Update()
    {
        if (numberOfItems[0] < 1) statsWall.GetComponent<TextMesh>().text = "Ściany: brak budulca" ;
        else statsWall.GetComponent<TextMesh>().text = "Ściany: " + numberOfItems[0].ToString();

        if (numberOfItems[1] < 1) statsRoof.GetComponent<TextMesh>().text = "Dachy: brak budulca";
        else statsRoof.GetComponent<TextMesh>().text = "Dachy: " + numberOfItems[1].ToString();

        if (numberOfItems[2] < 1) statsDoor.GetComponent<TextMesh>().text = "Drzwi: brak budulca";
        else statsDoor.GetComponent<TextMesh>().text = "Drzwi: " + numberOfItems[2].ToString();

        if (numberOfItems[3] < 1) statsWindow.GetComponent<TextMesh>().text = "Okna: brak budulca";
        else statsWindow.GetComponent<TextMesh>().text = "Okna: " + numberOfItems[3].ToString();

        if (Input.GetKeyUp("up") && !build && !putback)
        {
            build = true;
        }
        else if (Input.GetKeyUp("down"))
        {
            if(!build && !putback)
            {
                if (built.Count > 0) putback = true;
            }
           
        }
        else if (Input.GetKey("1"))
        {
            ITEM = 1;
        }
        else if (Input.GetKey("0"))
        {
            ITEM = 0;
        }
        else if (Input.GetKey("2"))
        {
            ITEM = 2;
        }
        else if (Input.GetKey("3"))
        {
            ITEM = 3;
        }
        else if (Input.GetKeyUp("right"))
        {
            if (X >= 3) return;
            X += 1;
            return;
        }
        else if (Input.GetKeyUp("left"))
        {
            if (X <= -4) return;
            X -= 1;
            return;
        }

        if (build && !putback)
        {
            if (!inWorkshop && !putback)
            {
                takeFromWorkshop(ITEM);
            }
            else if (inWorkshop && !putback)
            {
                moveToCoords(X,Y);
            }
        }
        else if (putback && !build) {
            putBackToWorkshop();
        }
        
        
    }
    
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
        
}
