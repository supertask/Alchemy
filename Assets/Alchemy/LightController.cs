using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;
using Leap.Unity;
using static Leap.Finger;
using UnityEngine.Experimental.VFX;

public class LightController : MonoBehaviour
{
    public GameObject mappedPoints;
    public GameObject leapProviderObj;
    public Transform player;
    public GameObject lineParentObj;
    public GameObject lineOriginObj;
    public GameObject lightParentObj;
    public List<GameObject> lightOriginObjs;

    private LeapServiceProvider m_Provider;
    private HandUtil handUtil;
    private bool isStartedLighting;
    private Util.Timer lineTimer;
    private Util.Timer light3Timer; //3番目のライト（LightOrigin3）を点灯させるタイマー
    private static readonly float LINE_DRAWING_SPEED = 0.012f;

    public struct Line {
        public Vector3 currentPoint;
        public Vector3 nextPoint;
        public float progress; //0 to 1
        public float drawingSpeed;
        public LineRenderer lineRenderer;
        public bool isStarted;
    }
    private Line line;
    private List<Vector3> inactivePoints;

    void Start()
    {
        this.m_Provider = this.leapProviderObj.GetComponent<LeapServiceProvider>();
        this.handUtil = new HandUtil(player);
        FingerMapper.Load(this.mappedPoints);

        this.isStartedLighting = false;
        this.inactivePoints = new List<Vector3>();
        foreach (Transform point in mappedPoints.transform) {
            this.inactivePoints.Add(point.position);
        }

        //線を描画する処理に使う
        this.line = new Line() {
            currentPoint = Vector3.zero, nextPoint = Vector3.zero, progress = 0.0f,
            drawingSpeed = LINE_DRAWING_SPEED
        };
        this.lineTimer = new Util.Timer(2.5f);
        this.light3Timer = new Util.Timer(6.0f);
    }

    void Update()
    {
        Frame frame = this.m_Provider.CurrentFrame;
        Hand[] hands = HandUtil.GetCorrectHands(frame); //0=LEFT, 1=RIGHT

        if (hands[HandUtil.LEFT] != null || hands[HandUtil.RIGHT] != null)
        {
            Hand leftHand = hands[HandUtil.LEFT];
            Hand rightHand = hands[HandUtil.RIGHT];
            bool isJustOpenedLeftHand = this.handUtil.JustOpenedHandOn(hands, HandUtil.LEFT);
            bool isJustOpenedRightHand = this.handUtil.JustOpenedHandOn(hands, HandUtil.RIGHT);
            if (!this.isStartedLighting && (isJustOpenedLeftHand || isJustOpenedRightHand))
            {
                Debug.Log("Opend a hand.");
                // 現在の光の位置と，次の光の位置を割り出す
                if (isJustOpenedLeftHand) {
                    this.line.currentPoint = HandUtil.ToVector3(leftHand.PalmPosition);
                }
                else if (isJustOpenedRightHand) {
                    this.line.currentPoint = HandUtil.ToVector3(rightHand.PalmPosition);
                }
                this.line.nextPoint = GetClosePoint(this.line.currentPoint);

                //this.TurnOnLight(1); // Light1をつける
                //this.TurnOnLight(2); // Light2をつける
                //this.light3Timer.Start(); //Light3をつけるまで待つ
                //this.lineTimer.Start(); //線の描画を待つ

                this.isStartedLighting = true;
                //Debug.Log(this.line.currentPoint + ", " + this.line.nextPoint);
            }
        }

        //時間が来たらlight3を点灯させる
        if (this.light3Timer.OnTime()) {
            this.TurnOnLight(3); // Light3をつける
            this.MakeLineObj(); //source pos=0, dest pos=0
        }

        /* // ある程度光が放たれ，線を描画するタイミングが来たら，線を作成
        if (this.lineTimer.OnTime()) {
        } */

        if (this.line.isStarted)
        {
            if (this.line.progress < 1.0f) {
                //線描画が始まっていたら，線を放っていく
                this.ProgressLight();
            }
            else {
                if (this.inactivePoints.Count > 0) {
                    //次のポイントに変更
                    this.line.currentPoint = this.line.nextPoint;
                    this.line.nextPoint = this.GetClosePoint(this.line.currentPoint);
                    this.TurnOnLight(1); // Light1をつける
                    this.TurnOnLight(2); // Light2をつける
                    this.light3Timer.Start(); //Light3をつけるまで待つ

                    this.line.progress = 0.0f;
                    this.line.isStarted = false;
                }
            }
        }


        if (this.light3Timer.isStarted) { this.light3Timer.Clock(); }
        //if (this.lineTimer.isStarted) { this.lineTimer.Clock(); }
    }

    private void ProgressLight()
    {
        this.line.progress += this.line.drawingSpeed;
        Vector3 midPoint = Vector3.Lerp(this.line.currentPoint, this.line.nextPoint, this.line.progress);
        this.line.lineRenderer.SetPosition(0, this.line.currentPoint);
        this.line.lineRenderer.SetPosition(1, midPoint);
    }

    //light_typeは1オリジンのため，1,2,3と指定する
    private void TurnOnLight(int light_type)
    {
        // Lightをつける
        GameObject newLight = Instantiate(this.lightOriginObjs[light_type-1]) as GameObject;
        newLight.GetComponent<VisualEffect>().enabled = true;
        newLight.transform.position = this.line.currentPoint;
        newLight.transform.parent = this.lightParentObj.transform;
    }

    private void MakeLineObj()
    {
        //線を引く
        GameObject newLine = Instantiate(this.lineOriginObj) as GameObject;
        newLine.transform.parent = this.lineParentObj.transform;
        this.line.lineRenderer = newLine.GetComponent<LineRenderer>();
        this.line.lineRenderer.enabled = true;
        this.line.isStarted = true;
    }



    Vector3 GetClosePoint(Vector3 sourcePoint)
    {
        float shortestDistance = 100000.0f;
        int closePointIndex = -1;

        for (int i = 0; i < this.inactivePoints.Count; i++) {
            float d = Vector3.Distance(sourcePoint, this.inactivePoints[i]);
            if (d < shortestDistance) {
                shortestDistance = d;
                closePointIndex = i;
            }
        }
        Debug.Log("i = " + closePointIndex);
        Vector3 closePoint = this.inactivePoints[closePointIndex]; //indexで落ちる
        this.inactivePoints.RemoveAt(closePointIndex);
        return closePoint;
    }


}
