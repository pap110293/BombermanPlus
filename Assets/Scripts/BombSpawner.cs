using UnityEngine;
using UnityEngine.Tilemaps;

public class BombSpawner : MonoBehaviour
{

    public Tilemap tilemap;
    public GameObject bombPrefab;
    public float moveSpeed = 1f;
    public int level = 1;

    private Camera mainCamera;
    private Rigidbody2D rb;

    private void Awake()
    {
        mainCamera = Camera.main;
        rb = GetComponent<Rigidbody2D>();
        InitPosition();
    }

    private void InitPosition()
    {
        var cell = tilemap.WorldToCell(transform.position);
        var cellCenterPos = tilemap.GetCellCenterWorld(cell);
        transform.position = cellCenterPos;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space))
            CreateABomb();

        var x = Input.GetAxis("Horizontal");
        var y = Input.GetAxis("Vertical");
        if (Mathf.Abs(x) + Mathf.Abs(y) > 0)
            Move(x, y);
    }

    public void Die()
    {
        // TODO: player die
        Debug.Log("die");
    }

    private void Move(float horizontal, float vertical)
    {
        Vector3 newPos = transform.position;
        newPos.x += horizontal * moveSpeed * Time.deltaTime;
        newPos.y += vertical * moveSpeed * Time.deltaTime;
        transform.position = newPos;

        // TODO: need other way to move
    }

    private void CreateABomb()
    {
        var cell = tilemap.WorldToCell(transform.position);
        var cellCenterPos = tilemap.GetCellCenterWorld(cell);
        var bomb = BombManager.CreateABomb(bombPrefab, cellCenterPos);
        //var bomb = Instantiate(bombPrefab, cellCenterPos, Quaternion.identity).GetComponent<Bomb>();
        bomb.SetLevel(level);
    }
}
