(function (global, factory) {
    if (typeof define === "function" && define.amd) {
        define(["exports", "core-js", "PIXI"], factory);
    } else if (typeof exports !== "undefined") {
        factory(exports, require("core-js"), require("PIXI"));
    } else {
        var mod = {
            exports: {}
        };
        factory(mod.exports, global.coreJs, global.PIXI);
        global.game = mod.exports;
    }
})(this, function (exports, _coreJs, _PIXI) {
    "use strict";

    Object.defineProperty(exports, "__esModule", {
        value: true
    });
    exports.drag = exports.isShot = exports.bulletRot = exports.obsticle = exports.obsticleTexture = exports.bullet = exports.bulletTexture = exports.tank = exports.tankTexture = exports.stage = exports.gameDiv = exports.renderer = exports.options = undefined;
    exports.shoot = shoot;
    exports.move = move;
    exports.isHit = isHit;
    exports.checkBullet = checkBullet;
    exports.animate = animate;

    var core_js = _interopRequireWildcard(_coreJs);

    var PIXI = _interopRequireWildcard(_PIXI);

    function _interopRequireWildcard(obj) {
        if (obj && obj.__esModule) {
            return obj;
        } else {
            var newObj = {};

            if (obj != null) {
                for (var key in obj) {
                    if (Object.prototype.hasOwnProperty.call(obj, key)) newObj[key] = obj[key];
                }
            }

            newObj.default = obj;
            return newObj;
        }
    }

    core_js;
    var options = exports.options = {
        backgroundColor: 1087931,
        resolution: 1
    };
    var renderer = exports.renderer = PIXI.autoDetectRenderer(800, 600, options);
    var gameDiv = exports.gameDiv = document.getElementById("game");
    gameDiv.appendChild(renderer.view);
    var stage = exports.stage = new _PIXI.Container();

    var tankTexture = exports.tankTexture = _PIXI.Texture.fromImage("tank.png");

    var tank = exports.tank = new _PIXI.Sprite(tankTexture);
    tank.anchor.x = 0.5;
    tank.anchor.y = 0.8;
    tank.position.x = 30;
    tank.position.y = 570;

    var bulletTexture = exports.bulletTexture = _PIXI.Texture.fromImage("bullet.png");

    var bullet = exports.bullet = new _PIXI.Sprite(bulletTexture);
    bullet.anchor.x = 0.5;
    bullet.anchor.y = 0.5;
    bullet.position.x = 30;
    bullet.position.y = 570;

    var obsticleTexture = exports.obsticleTexture = _PIXI.Texture.fromImage("bullet.png");

    var obsticle = exports.obsticle = new _PIXI.Sprite(obsticleTexture);
    obsticle.anchor.x = 0.5;
    obsticle.anchor.y = 0.5;
    obsticle.position.x = 400;
    obsticle.position.y = 120;
    var bulletRot = exports.bulletRot = 0;
    var isShot = exports.isShot = false;
    var drag = exports.drag = 0;

    function shoot(b) {
        exports.isShot = isShot = b;
    }

    function move(e) {
        var keyCode = (e.keyCode + 0x80000000 >>> 0) - 0x80000000;

        if (keyCode === 65) {
            tank.rotation = tank.rotation - 0.1;
        }

        if (keyCode === 68) {
            tank.rotation = tank.rotation + 0.1;
        }

        if (keyCode === 32 ? true : keyCode === 87) {
            shoot(true);
            exports.bulletRot = bulletRot = -tank.rotation;
        }

        return null;
    }

    function isHit(a, b) {
        return ((a.position.x < b.position.x + b.width ? b.position.x < a.position.x + a.width : false) ? a.position.y < b.position.y + b.height : false) ? b.position.y < a.position.y + a.height : false;
    }

    function checkBullet(shot) {
        if (shot) {
            exports.drag = drag = drag + 0.1;
            bullet.position.x = bullet.position.x - 10 * Math.sin(bulletRot);
            bullet.position.y = bullet.position.y - 10 * Math.cos(bulletRot) + drag;
        }

        if (((bullet.position.y < 0 ? true : bullet.position.y > 600) ? true : bullet.position.x < 0) ? true : bullet.position.x > 800) {
            bullet.position.x = 30;
            bullet.position.y = 570;
            exports.isShot = isShot = false;
            exports.drag = drag = 0;
        }

        if (isHit(bullet, obsticle)) {
            obsticle.texture = _PIXI.Texture.fromImage("boom.png");
        }
    }

    document.addEventListener('keydown', function (e) {
        return move(e);
    });
    stage.addChild(tank);
    stage.addChild(bullet);
    stage.addChild(obsticle);

    function animate(dt) {
        window.requestAnimationFrame(function (delegateArg0) {
            animate(delegateArg0);
        });
        checkBullet(isShot);
        renderer.render(stage);
    }

    animate(0);
});
//# sourceMappingURL=game.js.map