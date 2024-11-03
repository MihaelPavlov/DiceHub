/* Author by EdsonYap */
let $widget = $("#widget");
let assistOn = false;
// drag js below
let idleTime = 0;
let idling;
let idleTimeUnit = 100; // time unit
let idleFadeTime = 10, // time to start fade
  idleShrinkTime = idleFadeTime * 2, // time to start shrink
  idleTotalTime = idleFadeTime + idleShrinkTime; // end interval

// menu var below
//TweenMax.globalTimeScale(0.8);
const $assistiveTouch = $(".assistiveTouch");

const $assistiveItems = $(".assistiveItems");
const $assistiveItemsUL = $(".assistiveItems ul");
const $atItem = $(".atItem");

let $atItemLength = $(".atItem").length;
// default angle and distance 90, update if odd length
// var $angle = 90; var $distance = 90;
let $angle = $atItemLength > 2 ? 120 : 90;
let $distance = $atItemLength > 3 ? 125 : 95;

let $startingAngle = -90 + $angle / 2;
let $slice = $angle / ($atItemLength - 1);

// idling animation
function menuStatusAnimation(value) {
  switch (value) {
    case "Fade":
      TweenMax.to($widget, 1, {
        opacity: 0.7,
        ease: Quad.easeInOut,
      });
      break;
    case "Shrink":
      TweenMax.to($widget, 1, {
        scaleX: 0.5,
        scaleY: 0.5,
        ease: Expo.easeInOut,
      });
      break;
    case "Stop":
      idleTime = 0;
      TweenMax.to($widget, 0, { opacity: 1, scaleX: 1, scaleY: 1 });
      break;
    default:
      idleTime = 0;
      TweenMax.to($widget, 0, { opacity: 1, scaleX: 1, scaleY: 1 });
  }
}

function idleCountdown() {
  idleTime = 0;
  var idleInterval = setInterval(function () {
    if (idleTime <= idleTotalTime) {
      if (!assistOn) {
        idleTime = idleTime + 1;
        idling = true;
      } else {
        clearInterval(idleInterval);
        idleTime = 0;
        idling = false;
      }
    } else {
      clearInterval(idleInterval);
      idleTime = 0;
      idling = false;
    }
    if (idleTime >= idleShrinkTime) {
      menuStatusAnimation("Shrink");
    } else if (idleTime >= idleFadeTime) {
      menuStatusAnimation("Fade");
    }
  }, idleTimeUnit);
}

// expand menu direction
function itemsMenuDirection(expandDirection) {
  $atItem.each(function (i) {
    // default toBottom
    switch (expandDirection) {
      case "toLeft":
        var $sa = $atItemLength >= 2 ? 45 : 30;

        var $angle = $sa + $slice * i;
        TweenMax.to($(this), 1, {
          rotation: -$angle,
        });
        TweenMax.to($(this).find("a"), 1, {
          rotation: $angle,
        });
        break;
      case "toRight":
        var $sa = $atItemLength >= 2 ? 45 : 30;

        var $angle = $sa + $slice * i;
        TweenMax.to($(this), 1, {
          rotation: $angle,
        });
        TweenMax.to($(this).find("a"), 1, {
          rotation: -$angle,
        });
        break;
      case "toTop":
        var $sa = 135;

        var $angle = $sa + $slice * i;
        TweenMax.to($(this), 1, {
          rotation: -$angle,
        });
        TweenMax.to($(this).find("a"), 1, {
          rotation: $angle,
        });
        break;
      case "toBottom":
        var $sa = -90;
        var $angle = $startingAngle + $slice * i;
        TweenMax.to($(this), 1, {
          rotation: $angle,
        });
        TweenMax.to($(this).find("a"), 1, {
          rotation: -$angle,
        });
        break;
      default:
        var $sa = -90;
        var $angle = $startingAngle + $slice * i;
        TweenMax.to($(this), 1, {
          rotation: $angle,
        });
        TweenMax.to($(this).find("a"), 1, {
          rotation: -$angle,
        });
    }
  });
}
itemsMenuDirection("toBottom");

var draggableConfig = {
  scroll: false,
  containment: ".wrapperFullSize",
  start: function (event, ui) {
    if (!idling) {
      idleCountdown();
    }
  },
  drag: function (event, ui) {
    menuStatusAnimation();
    idleTime = 0;
    idling = !idling;
    itemsMenuDirection("toBottom");
  },
  stop: function (event, ui) {
    var stageWidth = $(window).width() - $("#widget").width(),
      stageHeight = $(window).height() - $("#widget").width();
    function bufferTopBottom() {
      if (ui.position.top <= (stageHeight / 100) * 15) {
        TweenMax.to($widget, 0.2, {
          top: $distance,
        });
      } else if (ui.position.top <= (stageHeight / 100) * 85) {
      } else {
        TweenMax.to($widget, 0.2, {
          top: stageHeight - $distance,
        });
      }
    }

    if (ui.position.left < stageWidth / 2) {
      bufferTopBottom();
      TweenMax.to($widget, 0.2, {
        left: "5",
        transformOrigin: "0% 50%",
      });
      itemsMenuDirection("toLeft");
    } else {
      bufferTopBottom();
      TweenMax.to($widget, 0.2, {
        left: stageWidth - 5,
        transformOrigin: "100% 50%",
      });
      itemsMenuDirection("toRight");
    }
    if (!idling) {
      idleCountdown();
    } else {
    }
  },
};

$widget.draggable(draggableConfig);

$(document).ready(function () {
  if (!$("#widget").is(":visible")) {
    idleCountdown(); // default countdown
  }

  // check rotate and reset
  $(window).resize(function () {
    $(".wrapperAssistive").css({ top: "20%", left: "4px" });
  });
});

// menu js below
$(".assistiveTouch, .overlayAssistive").on("click", assistiveHandler);

function assistiveHandler() {
  if ($assistiveTouch.closest("#widget").is(".ui-draggable-dragging")) {
    return;
  } else {
    event.preventDefault();
    event.stopPropagation();
    idleCountdown();
    assistOn = !assistOn;

    TweenMax.to($widget, 0.1, {
      opacity: 1,
      scaleX: 1,
      scaleY: 1,
      ease: Elastic.easeInOut,
    });

    TweenMax.fromTo(
      $assistiveTouch,
      0.2,
      {
        transformOrigin: "50% 50%",
      },
      {
        delay: 0.08,
        scaleX: 0.8,
        scaleY: 1.2,
        force3D: true,
        ease: Quad.easeInOut,
        onComplete: function () {
          TweenMax.to($assistiveTouch, 0.15, {
            scaleX: 1.2,
            scaleY: 0.8,
            force3D: true,
            ease: Quad.easeInOut,
            onComplete: function () {
              TweenMax.to($assistiveTouch, 2, {
                scaleX: 1,
                scaleY: 1,
                force3D: true,
                ease: Elastic.easeOut,
                easeParams: [1.1, 0.12],
              });
            },
          });
        },
      }
    );

    TweenMax.to($assistiveTouch, 0.4, {
      scale: assistOn ? 1 : 1,
      ease: Quint.easeInOut,
      force3D: true,
    });
    assistOn ? openAssist() : closeAssist();
    $(window).resize(function () {
      closeAssist();
      assistOn = false;
      $(".wrapperAssistive").css({ top: "20%", left: "4px" });
    });
  }
}

function openAssist() {
  $(".overlayAssistive").stop(true, true).fadeIn();

  $atItem.each(function (i) {
    var delay = i * 0.08;

    var $bounce = $(this).children(".atItemBg");
    TweenMax.fromTo(
      $bounce,
      0.2,
      {
        transformOrigin: "50% 50%",
      },
      {
        delay: delay,
        scaleX: 0.8,
        scaleY: 1.2,
        force3D: true,
        ease: Quad.easeInOut,
        onComplete: function () {
          TweenMax.to($bounce, 0.15, {
            // scaleX:1.2,
            scaleY: 0.7,
            force3D: true,
            ease: Quad.easeInOut,
            onComplete: function () {
              TweenMax.to($bounce, 3, {
                // scaleX:1,
                scaleY: 0.8,
                force3D: true,
                ease: Elastic.easeOut,
                easeParams: [1.1, 0.12],
              });
            },
          });
        },
      }
    );

    TweenMax.to($(this).children(".btnAtItem"), 0.5, {
      delay: delay,
      y: $distance,
      force3D: true,
      ease: Quint.easeInOut,
    });
  });
}

function closeAssist() {
  $(".overlayAssistive").stop(true, true).fadeOut();
  $atItem.each(function (i) {
    var delay = i * 0.08;

    var $bounce = $(this).children(".atItemBg");
    TweenMax.fromTo(
      $bounce,
      0.2,
      {
        transformOrigin: "50% 50%",
      },
      {
        delay: delay,
        scaleX: 1,
        scaleY: 0.8,
        force3D: true,
        ease: Quad.easeInOut,
        onComplete: function () {
          TweenMax.to($bounce, 0.15, {
            // scaleX:1.2,
            scaleY: 1.2,
            force3D: true,
            ease: Quad.easeInOut,
            onComplete: function () {
              TweenMax.to($bounce, 3, {
                // scaleX:1,
                scaleY: 1,
                force3D: true,
                ease: Elastic.easeOut,
                easeParams: [1.1, 0.12],
              });
            },
          });
        },
      }
    );

    TweenMax.to($(this).children(".btnAtItem"), 0.3, {
      delay: delay,
      y: 0,
      force3D: true,
      ease: Quint.easeIn,
    });
  });
}

// tutorial js
function tutClose() {
  $(".wrapperAssistiveTut").stop(true, true).fadeOut();
  idleCountdown();
}
