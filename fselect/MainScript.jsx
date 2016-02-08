$._ext = {
    addVideo: function(path) {
	if(app.project.activeItem == null){
		alert('コンポが選択されていません');
		return;
	}

	var io = new ImportOptions(File(path));
	if (io.canImportAs(ImportAsType.FOOTAGE)){
	    io.importAs = ImportAsType.FOOTAGE;
	    
	    io.sequence = false;
	    io.forceAlphabetical = true;
	    try{
	        seq = app.project.importFile(io);
	        comp = app.project.activeItem;
	        comp.layers.add(seq)
	    }
	    catch (e){
	        alert('コンポが選択されていません');
		return;
	    }

	}
    }
};