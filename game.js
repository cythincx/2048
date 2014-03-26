function Game(){

	var tileDiv = new Array();
	var bgColor = ['#CCC0B4','#EEE4DA','#EEDFCA','#F2B179','#EC8D55','#F57C5F','#EA593A','#EDCF72','#F1D04B','#ECC850','#E3BA14','#ECC400'];
	var textColor = [ '#776E65','#F9F6F2'];
	var biggestTile = 1;
	var tileNum = 2;

	var initial = function(){
		for( var i = 0 ; i < 4 ; i++ ){
			tileDiv[i] = [1,1,1,1];
		}
		addNew();
		addNew();
		renderTable();
		$("body").bind("keydown",function(event){moveAndJoin(event);});
	};

	var randomNum = function(){
		var x = Math.random();
		if(x<0.8) return 2;
		else return 4;
	};

	var randomTile = function(){
		return Math.floor(Math.random()*4);
	};

	var addNew = function(){
		var x = randomTile();
		var y = randomTile();
		var num = randomNum();
		while(tileDiv[x][y]!==1){
			x = randomTile();
			y = randomTile();
		}
		tileDiv[x][y]=num;
		//alert(x+' '+y+' '+num);
	};

	var renderTable = function(){
		var id = '';
		for( var i = 0 ; i < 4 ; i++ ){
			for( var j = 0 ; j < 4 ; j++ ){
				id= '#row' + i + 'col' + j;
				//alert(id);
				//if(tileDiv[i][j]!==1){
					//alert('cdsfdsa');
					$(id).html(tileDiv[i][j]!==1?tileDiv[i][j]:"");
					if(tileDiv[i][j]>1023){
						$(id).css('fontSize','30px');
					}else{
						$(id).css('fontSize','55px');
					}
					var textcolor = tileDiv[i][j]<5?0:1;
					$(id).css('color',textColor[textcolor]);
					var index = (Math.log(tileDiv[i][j]))/(Math.log(2));
					$(id).css('backgroundColor',bgColor[index]);
				//}
			}
		}
	};

	var moveAndJoin = function(event){
		var dir = event.keyCode;
		//alert(dir);
		switch(dir){
			case 37://left
			if(changeTileDiv(37)){
				addNew();
				renderTable();
				tileNum++;
			}
			//alert('left');
			break;
			case 38://top
			if(changeTileDiv(38)){
				addNew();
				renderTable();
				tileNum++;
			}
			//alert('top');
			break;
			case 39://right
			if(changeTileDiv(39)){
				addNew();
				renderTable();
				tileNum++;
			}
			//alert('right');
			break;
			case 40://bottom
			if(changeTileDiv(40)){
				addNew();
				renderTable();
				tileNum++;
			}
			//alert('bottom');
			break;
		}
		$("#info").html($("#info").html()+tileNum+'  ');
		winOrOver();

	};

	var changeTileDiv = function(dir){
		var isChange = false;
		switch(dir){
			case 37:
			for(var i = 0 ; i < 4 ; i++ ){
				var end = 0;
				for(var j = 1 ; j < 4; j++){
						if(tileDiv[i][j]!==1){
							if(tileDiv[i][j]===tileDiv[i][end]){
								tileDiv[i][end]*=2;
								if(tileDiv[i][end]>biggestTile){
									biggestTile = tileDiv[i][end];
								}
								tileDiv[i][j]=1;
								end++;
								isChange = true;
								tileNum--;
							}else if(tileDiv[i][end]===1){
								tileDiv[i][end] = tileDiv[i][j];
								tileDiv[i][j]=1;
								isChange = true;
								
								//end++;
							}else if(end!==j-1){
								end++;
								tileDiv[i][end] = tileDiv[i][j];
								tileDiv[i][j]=1;
								isChange = true;
								
							}else{
								end++;
							}
						}
				}
			}
			return isChange;
			break;
			case 38:
			for(var i = 0 ; i < 4 ; i++){
				var end = 0 ; 
				for(var j = 1 ; j < 4 ; j++ ){
					if(tileDiv[j][i]!==1){
						if(tileDiv[j][i]===tileDiv[end][i]){
							tileDiv[end][i]*=2;
							if(tileDiv[i][end]>biggestTile){
									biggestTile = tileDiv[i][end];
								}
							tileDiv[j][i]=1;
							end++;
							isChange = true;
							tileNum--;
						}else if(tileDiv[end][i]===1){
							tileDiv[end][i]=tileDiv[j][i];
							tileDiv[j][i]=1;
							isChange = true;
							
						}else if(end!== j -1){
							end++;
							tileDiv[end][i] = tileDiv[j][i];
							tileDiv[j][i]=1;
							isChange = true;
							
						}else{
							end++;
						}
					}
				}
			}
			return isChange;
			break;
			case 39:
			//alert('right');
			for( var i = 0 ; i < 4 ; i++ ){
				var end = 3;
				for( var j = 2 ; j >=0 ; j--){
					if(tileDiv[i][j]!==1){
						if(tileDiv[i][j]===tileDiv[i][end]){
							tileDiv[i][end]*=2;
							if(tileDiv[i][end]>biggestTile){
									biggestTile = tileDiv[i][end];
								}
							tileDiv[i][j]=1;
							end--;
							isChange = true;
							tileNum--;
						}else if(tileDiv[i][end]===1){
							tileDiv[i][end]=tileDiv[i][j];
							tileDiv[i][j]=1;
							isChange=true;
							
						}else if(end!==j+1){
							end--;
							tileDiv[i][end] = tileDiv[i][j];
							tileDiv[i][j] = 1;
							isChange = true;
							
						}else{
							end--;
						}
					}
				}
			}
			return isChange;
			break;
			case 40:
			for(var i = 0 ; i < 4 ; i++){
				var end = 3 ; 
				for(var j = 2 ; j >= 0 ; j-- ){
					if(tileDiv[j][i]!==1){
						if(tileDiv[j][i]===tileDiv[end][i]){
							tileDiv[end][i]*=2;
							if(tileDiv[i][end]>biggestTile){
									biggestTile = tileDiv[i][end];
								}
							tileDiv[j][i]=1;
							end--;
							isChange = true;
							tileNum--;
						}else if(tileDiv[end][i]===1){
							tileDiv[end][i]=tileDiv[j][i];
							tileDiv[j][i]=1;
							isChange = true;
							
						}else if(end!== j + 1){
							end--;
							tileDiv[end][i] = tileDiv[j][i];
							tileDiv[j][i]=1;
							isChange = true;
							
						}else{
							end--;
						}
					}
				}
			}
			return isChange;
			break;
		}
	};


	var winOrOver = function(){
		if(biggestTile===2048){
			$("body").unbind("keydown");
			$("#result").fadeIn(300);
			$("#resultText").fadeIn(300);
		}
		if(tileNum===16){
			//alert('tileNum = 16');
			if(isOver()){
				$("body").unbind("keydown");
				$("#result").css('backgroundColor','#eee');
				$("#resultText").css('color','#776E65');
				$("#resultText").html('Game over!');
				$("#result").fadeIn(300);
				$("#resultText").fadeIn(300);
			}
			
		}

	};

	var isOver = function(){
		var x = [0,-1,0,1];
		var y = [-1,0,1,0];
		for(var i = 0 ; i < 4 ; i++ ){
			for(var j = 0 ; j < 4 ; j++ ){
				for(var k = 0 ; k < 4 ; k++ ){
					var neighborX = i + x[k];
					if(neighborX<0||neighborX>3) continue;
					var neighborY = j + y[k];
					if(neighborY<0||neighborY>3) continue;
					if(tileDiv[neighborX][neighborY]===tileDiv[i][j]) return false; 
				}
			}
		}
		return true;
	};

	
	this.startGame = function(){
		initial();
		
	}
}

//alert('hello');


var game = new Game(); 
game.startGame();


			
