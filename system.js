function inherit(p){
	if(p==null) throw TypeError();
	if(Object.create){
		return Object.create(p);
	}
	vat t = typeof p;
	if(t!=='Object'&&t!=='function')
		throw TypeError();
	function f(){};
	f.prototype = p;
	return new f();
}
