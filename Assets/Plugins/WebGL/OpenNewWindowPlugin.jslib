var OpenNewWindowPlugin = {
//�N���b�N�C�x���g��o�^�F�V�����E�B���h�E
WindowOpenEventOn: function(openURL){
var url = Pointer_stringify(openURL);document.onmousedown = function(){
window.open(url, 'tweetWindow');
document.onmousedown = null;
} 
},
//�N���b�N�C�x���g��o�^�F�|�b�v�A�b�v�E�B���h�E
WindowPopUpEventOn: function(openURL){
var url = Pointer_stringify(openURL);document.onmousedown = function(){
window.open(url, 'tweetWindow','left=100, top=100, width=400, height=300, menubar=no, toolbar=no, scrollbars=yes');
document.onmousedown = null;
}
}, 
//�N���b�N�C�x���g���폜
WindowEventOff: function(){
document.onmousedown = null;
}
};

mergeInto(LibraryManager.library, OpenNewWindowPlugin);