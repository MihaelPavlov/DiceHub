import{Ea as y,Kc as T,M as a,Nc as l,Pa as c,Qa as F,S as f,T as s,Vc as x,fa as u,ia as m,ja as h,oa as g,pb as C,qa as d,wa as r,xa as n,ya as v}from"./chunk-JJGLVIRE.js";function M(e,t){if(e&1&&(r(0,"span"),c(1),n()),e&2){let I=t.$implicit;m(),F(I)}}var S=(()=>{let t=class t{constructor(o){this.router=o,this.imgPath="assets/img/401_error.svg",this.details=["The access to this page is restricted.","Please go to the Landing Page or refer to your system administrator."]}redirectTo(){this.router.navigateByUrl("/games/library")}};t.\u0275fac=function(i){return new(i||t)(h(T))},t.\u0275cmp=f({type:t,selectors:[["app-unauthorized"]],decls:9,vars:2,consts:[[1,"error-wrapper"],[1,"error-code-icon",3,"src"],[1,"container"],[1,"error-header"],[1,"content"],[4,"ngFor","ngForTrackByIndex","ngForOf"],[1,"mt-5",3,"click"]],template:function(i,p){i&1&&(r(0,"div",0),v(1,"img",1),r(2,"div",2)(3,"div",3),c(4,"Unauthorized"),n(),r(5,"div",4),g(6,M,2,1,"span",5),n()(),r(7,"button",6),y("click",function(){return p.redirectTo()}),c(8,"Navigate to Games Library"),n()()),i&2&&(m(),d("src",p.imgPath,u),m(5),d("ngForOf",p.details))},dependencies:[C],encapsulation:2});let e=t;return e})();var b=[{path:"",component:S}],j=(()=>{let t=class t{};t.\u0275fac=function(i){return new(i||t)},t.\u0275mod=s({type:t}),t.\u0275inj=a({imports:[l.forChild(b),l]});let e=t;return e})();var R=(()=>{let t=class t{};t.\u0275fac=function(i){return new(i||t)},t.\u0275mod=s({type:t}),t.\u0275inj=a({imports:[j,x]});let e=t;return e})();export{R as UnauthorizedModule};
