#r "../../node_modules/fable-core/Fable.Core.dll"
#load "../../node_modules/fable-import-pixi/Fable.Import.Pixi.fs"

open System
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import.PIXI
open Fable.Import.Browser

importAll "core-js"

module screenSize =
    let width   = 1024.
    let height  = 600.

let renderer =
  Globals.autoDetectRenderer( screenSize.width, screenSize.height)
  |> unbox<SystemRenderer>

let gameDiv = document.getElementById("game")
gameDiv.appendChild( renderer.view )

//Loading textures and initializing game objects

let stage = Container()

let back = Sprite(Texture.fromImage("img/back.png"))
back.position.x <- 0.
back.position.y <- 0.

let mutable currentPlayer = "Tank1"

module vars =
    let menuWidth = 459.
    let gameOverWidth = 387.
    let tankWidth = 74.
    let barrelWidth = 55.
    let pinWidth = 26.

module Menu =
    let logo = Sprite(Texture.fromImage("img/logo.png"))
    logo.position.x <- (screenSize.width - vars.menuWidth) / 2.
    logo.position.y <- 75.

    let gameOver = Sprite(Texture.fromImage("img/gameOver.png"))
    gameOver.position.x <- (screenSize.width - vars.gameOverWidth) / 2.
    gameOver.position.y <- 75.
    gameOver.visible <- false

module Tank1 =
    let body = Sprite(Texture.fromImage("img/tank.png"))
    body.position.x <- 10.
    body.position.y <- 520.

    let barrel = Sprite(Texture.fromImage("img/barrel.png"))
    barrel.anchor.x <- 0.1
    barrel.anchor.y <- 0.5
    barrel.position.x <- 60.
    barrel.position.y <- 525.

    let pin = Sprite(Texture.fromImage("img/p1.png"))
    pin.position.x <- 35.
    pin.position.y <- 485.

module Tank2 =
    let body = Sprite(Texture.fromImage("img/tank.png"))
    body.position.x <- screenSize.width - vars.tankWidth - 10.
    body.position.y <- 520.

    let barrel = Sprite(Texture.fromImage("img/barrel.png"))
    barrel.anchor.x <- 0.9
    barrel.anchor.y <- 0.5
    barrel.position.x <- screenSize.width - vars.barrelWidth - 10.
    barrel.position.y <- 525.

    let pin = Sprite(Texture.fromImage("img/p2.png"))
    pin.position.x <- screenSize.width - vars.pinWidth - 35.
    pin.position.y <- 485.
    pin.visible <- false

let bullet = Sprite(Texture.fromImage("img/bullet.png"))
bullet.anchor.x <- 0.5
bullet.anchor.y <- 0.5
bullet.position.x <- 55.
bullet.position.y <- 525.

//mutable variables for changing states

let mutable bulletRot = 0.
let mutable isShot = false
let mutable drag = 0.
let mutable gameStarted = false
let mutable gameOver = false

//functions without parameters are called only once during game lifecycle
//for that reason I always used a parameter if necessary
let shoot b = isShot <- b

//selecting current player
let selectTank name =
    if name = "Tank1" then
        Tank1.body, Tank1.barrel
    else
        Tank2.body, Tank2.barrel

//sound helper, taken from Fable.io pacman example
[<Emit("(new Audio($0)).play();")>]
let sound(file:string) : unit = failwith "never"

//keyboard controls
let move (e : KeyboardEvent) =
    let keyCode = int e.keyCode

    if  not isShot && gameStarted && not gameOver then
        let barrel = snd (selectTank currentPlayer)
        
        if currentPlayer = "Tank1" then
            if keyCode = 65 && barrel.rotation > -1.5 then 
                barrel.rotation <- barrel.rotation - 0.05

            if keyCode = 68 && barrel.rotation < 0. then
                barrel.rotation <- barrel.rotation + 0.05
        else
            if keyCode = 65 && barrel.rotation > 0. then 
                barrel.rotation <- barrel.rotation - 0.05

            if keyCode = 68 && barrel.rotation < 1.5 then
                barrel.rotation <- barrel.rotation + 0.05

        if keyCode = 32 then 
            shoot true
            sound("snd/shoot.wav")

            if currentPlayer = "Tank1" then
                currentPlayer <- "Tank2"
                bulletRot <- -barrel.rotation - 1.5
            else
                currentPlayer <- "Tank1"
                bulletRot <- -barrel.rotation + 1.5
    else if keyCode = 32 && not gameStarted then
        Menu.logo.visible <- false
        gameStarted <- true
    else if keyCode = 32 && gameOver then   //reset game states to initials
        gameStarted <- true
        gameOver <- false
        Menu.gameOver.visible <- false
        Tank1.barrel.rotation <- 0.
        Tank2.barrel.rotation <- 0.
        Tank1.body.texture <- Texture.fromImage("img/tank.png")
        Tank2.body.texture <- Texture.fromImage("img/tank.png")
        bullet.position.x <- 55.
        bullet.position.y <- 525.
        Tank1.pin.visible <- true
        Tank2.pin.visible <- false
        isShot <- false
        bulletRot <- 0.
        currentPlayer <- "Tank1"
        
    null

//check hit of a smaller rectangle in tank body texture
let isHit (bullet:Sprite) (tank:Sprite) =
    bullet.position.x     < tank.position.x + tank.width - 10. &&
    tank.position.x + 10. < bullet.position.x + bullet.width   &&
    bullet.position.y     < tank.position.y + tank.height      &&
    tank.position.y + 10. < bullet.position.y + bullet.height

//mark current player
let changePin name =
    if name = "Tank1" then
        Tank1.pin.visible <- true
        Tank2.pin.visible <- false
    else
        Tank1.pin.visible <- false
        Tank2.pin.visible <- true

//checking behavior of bullet
let checkBullet shot =
    if shot then
        drag <- drag + 0.1
        bullet.position.x <- bullet.position.x - (10. * Math.Sin(bulletRot)) 
        bullet.position.y <- bullet.position.y - (10. * Math.Cos(bulletRot)) + drag

    if bullet.position.y < 0. || bullet.position.y > screenSize.height - 55. || bullet.position.x < 0. || bullet.position.x > screenSize.width then
        if currentPlayer = "Tank1" then
            bullet.position.x <- 55.
            bullet.position.y <- 525.
        else
            bullet.position.x <- screenSize.width - vars.tankWidth + 5.
            bullet.position.y <- 525.

        changePin currentPlayer

        isShot <- false
        drag <- 0.

    let body = selectTank currentPlayer |> fst

    if isHit bullet body then
        body.texture <- Texture.fromImage("img/tankHit.png")
        gameOver <- true
        Menu.gameOver.visible <- true
        sound("snd/die.wav")

document.addEventListener_keydown(fun e -> move(e))

//sound("snd/loop.wav")

//adding all game objects to screen
stage.addChild(back)            |> ignore
stage.addChild(bullet)          |> ignore
stage.addChild(Tank1.barrel)    |> ignore
stage.addChild(Tank1.body)      |> ignore
stage.addChild(Tank1.pin)       |> ignore
stage.addChild(Tank2.barrel)    |> ignore
stage.addChild(Tank2.body)      |> ignore
stage.addChild(Tank2.pin)       |> ignore
stage.addChild(Menu.logo)       |> ignore
stage.addChild(Menu.gameOver)   |> ignore

//game update method
let rec animate (dt:float) =
    window.requestAnimationFrame(FrameRequestCallback animate) |> ignore

    checkBullet isShot

    renderer.render(stage)

animate 0.