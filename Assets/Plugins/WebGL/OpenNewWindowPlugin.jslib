var OpenNewWindowPlugin = {
//クリックイベントを登録：新しいウィンドウ
WindowOpenEventOn: function(openURL){
var url = Pointer_stringify(openURL);document.onmousedown = function(){
window.open(url, 'tweetWindow');
document.onmousedown = null;
} 
},
//クリックイベントを登録：ポップアップウィンドウ
WindowPopUpEventOn: function(openURL){
var url = Pointer_stringify(openURL);document.onmousedown = function(){
window.open(url, 'tweetWindow','left=100, top=100, width=400, height=300, menubar=no, toolbar=no, scrollbars=yes');
document.onmousedown = null;
}
}, 
//クリックイベントを削除
WindowEventOff: function(){
document.onmousedown = null;
}
};

mergeInto(LibraryManager.library, OpenNewWindowPlugin);