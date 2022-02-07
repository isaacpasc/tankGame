using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    //CREATE ENEMY:
    public enum tankSpeed
    {
        stationary,
        slow,
        normal,
        fast,
        veryFast,
        lightning
    }
    public tankSpeed enemyTankSpeed;

    public enum ai
    {
        stationary,
        patrol,
        chase,
        kamikaze
    }
    public ai enemyAi;

    public enum bulletSpeed
    {
        slow,
        normal,
        fast,
        veryFast,
        laserFast
    }
    public bulletSpeed enemyBulletSpeed;

    public enum weapon
    {
        normal,
        shotgun,
        rocket,
        bomb
    }
    public weapon enemyWeaopn;

    //FOV:
    [SerializeField] private float viewAngle;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private LayerMask obstacleMask;

    //shooting:
    [SerializeField] private Transform firePoint;
    [SerializeField] private Transform firePoint1;
    [SerializeField] private Transform firePoint2;
    [SerializeField] private Transform muzzleFlashTransform;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject rocketPrefab;
    private float bulletForce = 0f;
    [SerializeField] private float shotPeriod = .25f;
    [SerializeField] private Animator shootAnimation;
    private float shotTimer = 0f;
    [SerializeField] private GameObject enemyDeathPrefab;
    [SerializeField] private GameObject player;

    //LOAD SPRITES:
    [Header("Load Track Sprites")]
    [SerializeField] private SpriteRenderer trackSprite;
    [SerializeField] private Animator tracks;
    [SerializeField] private Sprite stationaryTrack;
    [SerializeField] private Sprite slowTrack;
    [SerializeField] private Sprite normalTrack;
    [SerializeField] private Sprite fastTrack;
    [SerializeField] private Sprite veryFastTrack;
    [SerializeField] private Sprite lightningTrack;

    [Header("Load Hull Sprites")]
    [SerializeField] private SpriteRenderer hullSprite;
    [SerializeField] private Sprite stationaryHull;
    [SerializeField] private Sprite kamikazeHull;
    [SerializeField] private Sprite patrolHull;
    [SerializeField] private Sprite chaseHull;

    [Header("Load Muzzle Sprites")]
    [SerializeField] private SpriteRenderer muzzleSprite;
    [SerializeField] private Transform muzzleTransform;
    [SerializeField] private Animator muzzleAnimator;
    [SerializeField] private Sprite normalMuzzle;
    [SerializeField] private Sprite rocketMuzzle;
    [SerializeField] private Sprite shotgunMuzzle;

    [Header("Load Bullet Sprites")]
    [SerializeField] private Sprite rocketSprite;

    // patrol var's:
    private float waitTime = 3f;
    private float startWaitTime;
    private Vector2 movePosition;

    //pathfinding var's:
    private Path path;
    private int currWaypoint = 0;
    private bool reachedEndOfPath = false;
    private Seeker seeker;
    private Rigidbody2D rb;
    private float nextWaypointDistance = 0.5f;
    private bool inSight = false;

    //speed var's:
    float speed;


    void Awake()
    {
        //TANKS SPEED:

        //stationary:
        if(enemyTankSpeed == tankSpeed.stationary)
        {
            //animation
            tracks.SetBool("isStationary", true);
            tracks.enabled = false;

            //idle sprite
            trackSprite.sprite = stationaryTrack;

            //speed
            speed = 0f;
        }
        //slow:
        if (enemyTankSpeed == tankSpeed.slow)
        {
            //animation
            tracks.SetBool("isSlow", true);
            tracks.enabled = false;

            //idle sprite
            trackSprite.sprite = slowTrack;

            //speed
            speed = 2f;
        }
        //normal:
        if (enemyTankSpeed == tankSpeed.normal)
        {
            //animation
            tracks.SetBool("isNormal", true);
            tracks.enabled = false;

            //idle sprite
            trackSprite.sprite = normalTrack;

            //speed
            speed = 4f;
        }
        //fast:
        if (enemyTankSpeed == tankSpeed.fast)
        {
            //animation
            tracks.SetBool("isFast", true);
            tracks.enabled = false;

            //idle sprite
            trackSprite.sprite = fastTrack;

            //speed
            speed = 6f;
        }
        //very fast:
        if (enemyTankSpeed == tankSpeed.veryFast)
        {
            //animation
            tracks.SetBool("isVeryFast", true);
            tracks.enabled = false;

            //idle sprite
            trackSprite.sprite = veryFastTrack;

            //speed
            speed = 8f;
        }
        //lightning:
        if (enemyTankSpeed == tankSpeed.lightning)
        {
            //animation
            tracks.SetBool("isLightning", true);
            tracks.enabled = false;

            //idle sprite
            trackSprite.sprite = lightningTrack;

            //speed
            speed = 10f;
        }

        //Muzzle:

        //normal:
        if(enemyWeaopn == weapon.normal)
        {
            //muzzle sprite
            muzzleSprite.sprite = normalMuzzle;
        }
        //bomb:
        if (enemyWeaopn == weapon.bomb)
        {
            //muzzle sprite
            muzzleAnimator.enabled = true;
        } else
        {
            muzzleAnimator.enabled = false;
        }
        //rocket:
        if (enemyWeaopn == weapon.rocket)
        {
            //muzzle sprite
            muzzleSprite.sprite = rocketMuzzle;
            muzzleTransform.localScale = new Vector3(0.8f, 0.8f, 0f);
        }
        //shotgun:
        if (enemyWeaopn == weapon.shotgun)
        {
            //muzzle sprite
            muzzleSprite.sprite = shotgunMuzzle;
            firePoint.localPosition = new Vector3(0f, 2.5f, 0f);
            muzzleFlashTransform.localPosition = new Vector3(0f, 3f, 0f);
            muzzleTransform.localScale = new Vector3(1f, 0.8f, 0f);
        }

        //BULLET SPEED:

        //slow:
        if(enemyBulletSpeed == bulletSpeed.slow)
        {
            bulletForce = 10f;
        }
        //normal:
        if (enemyBulletSpeed == bulletSpeed.normal)
        {
            bulletForce = 15f;
        }
        //fast:
        if (enemyBulletSpeed == bulletSpeed.fast)
        {
            bulletForce = 20f;
        }
        //very fast:
        if (enemyBulletSpeed == bulletSpeed.veryFast)
        {
            bulletForce = 25f;
        }
        //laser fast:
        if (enemyBulletSpeed == bulletSpeed.laserFast)
        {
            bulletForce = 30f;
        }

        //AI:

        //stationary:
        if (enemyAi == ai.stationary)
        {
            //hull sprite
            hullSprite.sprite = stationaryHull;
        }
        //patrol:
        if (enemyAi == ai.patrol)
        {
            startWaitTime = waitTime;
            movePosition = moveSpot();
            //hull sprite
            hullSprite.sprite = patrolHull;
        }
        //chase:
        if (enemyAi == ai.chase)
        {
            seeker = GetComponent<Seeker>();
            rb = GetComponent<Rigidbody2D>();
            //hull sprite
            hullSprite.sprite = chaseHull;
        }
        //kamikaze:
        if (enemyAi == ai.kamikaze)
        {
            seeker = GetComponent<Seeker>();
            rb = GetComponent<Rigidbody2D>();
            //hull sprite
            hullSprite.sprite = kamikazeHull;
        }
    }

    void Start()
    {
        if (enemyAi == ai.chase || enemyAi == ai.kamikaze)
        {
            InvokeRepeating("updatePath", 0f, 0.5f);
        }
        StartCoroutine("FindTargetWithDelay", 0.1f);
    }
    void updatePath()
    {
        if (seeker.IsDone())
        {
            seeker.StartPath(rb.position, targetTransform.position, onPathComplete);
        }
    }
    void onPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currWaypoint = 0;
        }
    }
    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void Update()
    {
        if (enemyAi == ai.patrol)
        {
                transform.position = Vector2.MoveTowards(transform.position, movePosition, speed * Time.deltaTime);

                if (Vector2.Distance(transform.position, movePosition) < 0.2f)
                {
                tracks.enabled = false;
                    if (waitTime <= 0)
                    {
                        movePosition = moveSpot();
                        waitTime = startWaitTime;
                    } else
                    {
                        waitTime -= Time.deltaTime;
                    }
                } else
            {
                tracks.enabled = true;
            }
 
        }
    }

    void FixedUpdate()
    {
        if (enemyAi == ai.chase || enemyAi == ai.kamikaze)
        {
            if (path != null)
            {
                float dist2 = Mathf.Abs(Vector2.Distance(rb.position, targetTransform.position));
                if (inSight && enemyAi == ai.chase && dist2 < 4f)
                {
                    rb.velocity = new Vector2(0, 0);
                    tracks.enabled = false;
                    return;
                }
                if (enemyAi == ai.kamikaze)
                {
                    if (inSight && dist2 < 3f)
                    {
                        muzzleAnimator.speed = 5f;
                        if (dist2 < 2f)
                        {
                            //explode
                            GameObject enemyDeath = Instantiate(enemyDeathPrefab, gameObject.transform.position, gameObject.transform.rotation);
                            //Destroy(player);
                            Destroy(gameObject);
                        }
                    }
                }
                tracks.enabled = true;
                if (currWaypoint >= path.vectorPath.Count)
                {
                    reachedEndOfPath = true;
                }
                else
                {
                    reachedEndOfPath = false;
                }

                Vector2 direct = ((Vector2)path.vectorPath[currWaypoint] - rb.position).normalized;
                Vector2 force = direct * speed * Time.deltaTime;

                rb.velocity = direct * speed/2;

                float dist = Vector2.Distance(rb.position, path.vectorPath[currWaypoint]);

                if (dist < nextWaypointDistance)
                {
                    currWaypoint++;
                }
                if (rb.velocity.x > 0f || rb.velocity.y > 0f)
                {
                    float ang = Mathf.Atan2(path.vectorPath[currWaypoint].y - rb.position.y, path.vectorPath[currWaypoint].x - rb.position.x) * 180 / Mathf.PI - 90f;
                    transform.rotation = Quaternion.AngleAxis(ang, Vector3.forward);
                }
            }
        }
    }

    //Finds targets inside field of view not blocked by walls
    void FindVisibleTargets()
    {
        //Adds targets in view radius to an array
        Collider2D[] targetsInViewRadius = Physics2D.OverlapCircleAll(transform.position, 360, targetMask);
        //For every targetsInViewRadius it checks if they are inside the field of view
        for (int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;
            if (Vector3.Angle(transform.up, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);
                //If line draw from object to target is not interrupted by wall, add target to list of visible 
                //targets
                if (!Physics2D.Raycast(transform.position, dirToTarget, dstToTarget-3, obstacleMask))
                {

                    inSight = true;
                    if (enemyWeaopn != weapon.bomb)
                    {
                        //rotate muzzle to target player
                        float angle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg - 90f;
                        muzzleTransform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);

                        //shoot
                        if (shotTimer == 0f)
                        {
                            shoot();
                        }
                        else
                        {
                            shotTimer -= Time.deltaTime;
                            shotTimer = Mathf.Max(shotTimer, 0);
                        }
                    }
                } else
                {
                    inSight = false;
                }
            }
        }
    }


    Vector2 moveSpot()
    {
        Vector2 vect = transform.position;
        float minY;
        float minX;
        float maxY;
        float maxX;
        bool spotValid = false;
        while (!spotValid)
        {
            minY = transform.position.y - 20f;
            minX = transform.position.x - 20f;
            maxY = transform.position.y + 20f;
            maxX = transform.position.x + 20f;
            vect = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
            float dst = Vector2.Distance(transform.position, vect);
            Vector2 dir = (vect - new Vector2(transform.position.x, transform.position.y)).normalized;
            if (!Physics2D.Raycast(transform.position, dir, dst, obstacleMask)) 
            {
                spotValid = true;
            }
        }
        float ang = Mathf.Atan2(vect.y - transform.position.y, vect.x - transform.position.x) * 180 / Mathf.PI - 90f;
        transform.rotation = Quaternion.AngleAxis(ang, Vector3.forward);
        return vect;
    }

    void shoot()
    {
        if (enemyWeaopn == weapon.shotgun)
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            GameObject bullet1 = Instantiate(bulletPrefab, firePoint.position, firePoint1.rotation);
            GameObject bullet2 = Instantiate(bulletPrefab, firePoint.position, firePoint2.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            Rigidbody2D rb1 = bullet1.GetComponent<Rigidbody2D>();
            Rigidbody2D rb2 = bullet2.GetComponent<Rigidbody2D>();
            rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
            rb1.AddForce(firePoint1.up * bulletForce, ForceMode2D.Impulse);
            rb2.AddForce(firePoint2.up * bulletForce, ForceMode2D.Impulse);
        } else  if (enemyWeaopn == weapon.rocket)
        {
            GameObject bullet = Instantiate(rocketPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
        } else
        {
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(firePoint.up * bulletForce, ForceMode2D.Impulse);
        }
        shootAnimation.Play("RedMuzzleFlash");
        shotTimer = shotPeriod;
    }
}
