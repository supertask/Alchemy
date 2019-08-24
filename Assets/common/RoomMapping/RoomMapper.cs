using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;
using Dummiesman;

public class RoomMapper : MonoBehaviour
{
    public GameObject mapped;
    public GameObject selecting;
    public Transform player;
    public Material cubeMat;
    public Material objMat;

    public static readonly string SCENE_NAME = "RoomMapper";
    private string scannedObjPath;
    private int objIndex;
    private int boxIndex;
    private int selectingIndex;
    private static string SCANNED_OBJ_PREFIX = "ScannedObj";
    private static string CUBE_PREFIX = "Cube";

    void Start()
    {
        this.scannedObjPath = Application.streamingAssetsPath + "/ScannedObjs/";
        this.objIndex = 0;
        this.boxIndex = 0;
        this.selectingIndex = 0;
        this.Load();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == RoomMapper.SCENE_NAME)  {
            if (Input.GetKeyUp(KeyCode.O)) {

                Debug.Log("Key O");
                this.CreateScannedObject(); //マップするボックス作成
            }
            else if (Input.GetKeyUp(KeyCode.B)) {
                Debug.Log("Key B");
                this.CreateCube(); //マップするボックス作成
            }
            else if (Input.GetKeyUp(KeyCode.M)) {
                Debug.Log("Key M");
                this.SwitchSelectingObj();
            }
            else if (Input.GetKeyUp(KeyCode.E)) {
                Debug.Log("Key E");
                this.Save(); //マッピング終了
            }
        }
    }

    /*
     * Cubeを作成
     */
    public void CreateCube()
    {
        this.MapSelectingObjects(); //選択したオブジェクトをマッピングする
        GameObject box = GameObject.CreatePrimitive(PrimitiveType.Cube);
        box.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        box.transform.position = player.position +  player.rotation * Vector3.forward * 0.3f; //0.3m先
        box.GetComponent<Renderer>().enabled = true;
        box.transform.parent = this.selecting.transform;
    }

    /*
     * スキャンしたオブジェクトを作成し，選択中オブジェクトに加える
     */
    public void CreateScannedObject()
    {
        DirectoryInfo streamingAssets = new DirectoryInfo(this.scannedObjPath);
        FileInfo[] infos = streamingAssets.GetFiles("*.obj");
        if (this.objIndex < infos.Length) {
            FileInfo info = infos[this.objIndex];
            Debug.Log(info.FullName);

            this.MapSelectingObjects();
            GameObject newObj = new OBJLoader().Load(info.FullName);
            newObj.name = RoomMapper.SCANNED_OBJ_PREFIX + this.objIndex;
            newObj.transform.position = player.position + player.rotation * Vector3.forward * 1.5f; //1.5m先
            newObj.transform.parent = this.selecting.transform;
            this.objIndex++;
        }
        else {
            Debug.Log("ERROR: No more obj files.");
        }
    }

    /*
     * セーブデータを読み込む
     */
    public void Load()
    {
        /*
        bool is_shown_map = (SceneManager.GetActiveScene().name == RoomMapper.SCENE_NAME);

        if (PlayerPrefs.HasKey("roomMappedObjectTypeList"))
        {
            if (this.mapped.transform.childCount > 0)
            {
                foreach (Transform child in this.mapped.transform) { GameObject.Destroy(child.gameObject); }
            }
            string[] objTypeList = PlayerPrefsX.GetStringArray("roomMappedObjectTypeList");
            Vector3[] posList = PlayerPrefsX.GetVector3Array("roomMappedPosList");
            Quaternion[] rotList = PlayerPrefsX.GetQuaternionArray("roomMappedRotList");
            Vector3[] scaleList = PlayerPrefsX.GetVector3Array("roomMappedScaleList");
            for (int i = 0; i < posList.Length; i++)
            {
                if (objTypeList[i].IndexOf(RoomMapper.SCANNED_OBJ_PREFIX) >= 0) {
                    GameObject obj = Object.Instantiate(this.) as GameObject;
                }
                else if (objTypeList[i].IndexOf("Box") >= 0) {
                }
                obj.transform.position = posList[i];
                obj.transform.rotation = rotList[i];
                obj.transform.localScale = scaleList[i];
                obj.transform.parent = this.mapped.transform;
                obj.GetComponent<Renderer>().enabled = is_shown_map;
            }
        }
        else
        {
            //セーブデータがない場合，何もしない
        }
        */
    }

    /*
     * 選択中のオブジェクトをマップ済みオブジェクトへ登録する
     */
    private void MapSelectingObjects() {
        foreach(Transform child in this.selecting.transform) {
            child.parent = this.mapped.transform;
        }
    }

    /*
     * 選択中オブジェクトを切り替える
     */
    public void SwitchSelectingObj() {
        this.selectingIndex = (this.selectingIndex + 1) % this.mapped.transform.childCount;
        Transform selectedTransform = this.mapped.transform.GetChild(this.selectingIndex);
        selectedTransform.parent = this.selecting.transform;
    }


    /*
     * データをPlayerPrefsXに保存する
     */
    public void Save()
    {
        /*
        this.MapSelectingObjects(); //選択したオブジェクトをマッピングする
        List<string> savingObjTypeList = new List<string>() { };
        List<Vector3> savingPosList = new List<Vector3>() { };
        List<Quaternion> savingRotList = new List<Quaternion>() { };
        List<Vector3> savingScaleList = new List<Vector3>() { };

        foreach(Transform transform in this.mapped.transform) {
            savingObjTypeList.Add(transform.gameObject.name);
            savingPosList.Add(transform.position);
            savingRotList.Add(transform.rotation);
            savingScaleList.Add(transform.localScale);
        }
        PlayerPrefsX.SetStringArray("roomMappedObjectTypeList", savingObjTypeList.ToArray());
        PlayerPrefsX.SetVector3Array("roomMappedPosList", savingPosList.ToArray());
        PlayerPrefsX.SetQuaternionArray("roomMappedRotList", savingRotList.ToArray());
        PlayerPrefsX.SetVector3Array("roomMappedScaleList", savingScaleList.ToArray());
        */
    }

    void OnApplicationQuit()
    {
    }
}
