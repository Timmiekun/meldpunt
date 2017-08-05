function XHR(){
    try {
      return new XMLHttpRequest()
    }
    catch(e){
       return new ActiveXObject("Microsoft.XMLHTTP")
    }    
}