$(document).ready(function () {

    let uppy = new Uppy.Uppy({
        id: 'uppy',
        target: '#uppy',
        inline: true,
        replaceTargetContent: true,
        showProgressDetails: true,
        note: 'PDF Files only',
        restrictions: {
            allowedFileTypes :['.pdf']
        }
    })

    uppy.use(Uppy.Dashboard, {
        target: '#uppyDashboard',
        inline: true,
        height: 600,
        width: '100%',
        metaFields: [
            {id: 'name', name: 'Name', placeholder: 'file name'},
            {id: 'caption', name: 'Caption', placeholder: 'add description'},
        ],
        note: 'Upload .pdf files only',
    })
    uppy.use(Uppy.ImageEditor, {target: Uppy.Dashboard})
    uppy.use(Uppy.Form, {target: '#upload-form'})
    // Allow dropping files on any element or the whole document
    uppy.use(Uppy.Compressor)
    uppy.use(Uppy.XHRUpload, {
        endpoint: '/Supplier/Upload',
        formData: true,
        fieldName: 'files[]',
    })
    uppy.on('complete', result => {
        console.log('successful files:', result.successful)
        console.log('failed files:', result.failed)
        if (result.successful.length > 0) {
            window.location.href = '/Supplier/AcceptOrRejectData?uploadId=' + result.successful[0].response.body.uploadId
        }
    })

});