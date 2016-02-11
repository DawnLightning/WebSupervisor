 // JavaScript Document
function g(o){return document.getElementById(o);}
function hoverli(n){
for(var i=1;i<=11;i++){g('tb_'+i).className='aa';g('tbc_0'+i).className='undis';}g('tbc_0'+n).className='list2';g('tb_'+n).className='bb';
}
function fun2(){
hoverli2(3);
}
function hoverli3(n){
for(var i=1;i<=11;i++){g('be_'+i).className='aa';g('bec_0'+i).className='undis';}g('bec_0'+n).className='list2';g('be_'+n).className='bb';
}
function fun3(){
hoverli2(3);
}