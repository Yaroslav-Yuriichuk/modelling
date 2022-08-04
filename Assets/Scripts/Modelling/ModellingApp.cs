using System;
using System.Collections.Generic;
using DG.Tweening;
using Modelling.Commands;
using Modelling.Services;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using ILogger = Modelling.Services.ILogger;
using Random = UnityEngine.Random;

namespace Modelling
{
    public class ModellingApp : MonoBehaviour
    {
        [Header("Resolution")]
        [SerializeField] private Size3D _size;
        [SerializeField] private float _modelSize;

        [Header("Positions")]
        [SerializeField] private Transform _deformableModelPosition;
        [SerializeField] private Transform _targetModelPosition;

        [Header("Controls")]
        [SerializeField] private Button _generateButton;
        [SerializeField] private Button _regenerateButton;
        [SerializeField] private Button _exitButton;

        private const float ShowHideDuration = 1f;
        
        private ModelObject _deformableModel;
        private ModelObject _targetModel;

        private IModellingService _modellingService;
        private IInputService _inputService;
        private IModelGeneratingService _modelGeneratingService;
        private IIdentityService _identityService;
        private ILoadingService _loadingService;
        private ILogger _logger;
        
        [Inject]
        private void Construct(IInputService inputService, IModellingService modellingService,
            IModelGeneratingService modelGeneratingService, IIdentityService identityService,
            ILoadingService loadingService, ILogger logger,
            ModelObject deformableModel, ModelObject targetModel)
        {
            _modellingService = modellingService;
            _inputService = inputService;
            _modelGeneratingService = modelGeneratingService;
            _identityService = identityService;
            _loadingService = loadingService;
            _logger = logger;
            
            _deformableModel = deformableModel;
            _targetModel = targetModel;

            if (!_size.IsChunkable()) throw new ApplicationException("Model size must be evenly chunkable");
            
            _deformableModel.transform.SetParent(_deformableModelPosition, false);
            _deformableModel.Init(_size, true);
            
            _targetModel.transform.SetParent(_targetModelPosition, false);
            _targetModel.Init(_size, false);
            _targetModel.transform.localScale = Vector3.zero;
        }

        private void OnEnable()
        {
            _generateButton.onClick.AddListener(GenerateTargetModel);
            _regenerateButton.onClick.AddListener(GenerateTargetModel);
            _exitButton.onClick.AddListener(CloseApp);
        }

        private void OnDisable()
        {
            _generateButton.onClick.RemoveListener(GenerateTargetModel);
            _regenerateButton.onClick.RemoveListener(GenerateTargetModel);
            _exitButton.onClick.RemoveListener(CloseApp);
        }

        private void Start()
        {
            float voxelSize = _modelSize / Mathf.Max(_size.Width, _size.Height, _size.Length);
            _deformableModel.ApplyModel(_modelGeneratingService.GenerateModel(
                new ModelConstraints(ModelType.Cube, _size.Width, _size.Height, _size.Length, voxelSize)));
        }

        private void Update()
        {
            if (_targetModel.Model == null) return;
            if (_loadingService.IsLoading) return;
            
            if (_inputService.GetIntrusionPoint(out Vector3 point))
            {
                _logger.Log($"Intrusion point: {point}");
                _modellingService.AddCommand(new IntrudeCommand(_deformableModel, point));
                return;
            }

            if (_inputService.GetExtrusionPoint(out point))
            {
                _logger.Log($"Extrusion point: {point}");
                _modellingService.AddCommand(new ExtrudeCommand(_deformableModel, point));
                return;
            }

            if (_inputService.WantsToUndo())
            {
                _modellingService.UndoLastCommand();
            }
        }

        private void GenerateTargetModel()
        {
            const float normalizationInterval = 0.4f;
            const float delayBeforeGenerating = 0.65f;
            
            Sequence sequence = DOTween.Sequence();

            if (_targetModel.Model != null)
            {
                sequence.Append(_targetModel.transform.DOScale(Vector3.zero, ShowHideDuration));
                _regenerateButton.transform.DOScale(Vector3.zero, ShowHideDuration / 3);
            }
            else
            {
                sequence
                    .Append(_generateButton.transform.DOScale(Vector3.zero, ShowHideDuration / 4));
            }

            sequence
                .AppendCallback(_loadingService.Show)
                .AppendInterval(delayBeforeGenerating)
                .AppendCallback(() =>
                {
                    _targetModel.ApplyModel(_modelGeneratingService.GenerateModel(
                        new ModelConstraints(GetRandomModelType(), _size.Width, _size.Height, _size.Length,
                            _modelSize / _size.Width)));
                    
                    _identityService.SetTargetModel(_targetModel.Model);
                })
                .AppendInterval(normalizationInterval)
                .AppendCallback(_loadingService.Hide)
                .Append(_targetModel.transform.DOScale(Vector3.one, ShowHideDuration))
                .AppendCallback(() => _identityService.Calculate(_deformableModel.Model))
                .Append(_regenerateButton.transform.DOScale(Vector3.one, ShowHideDuration / 3));
            
            sequence.Play();
        }

        private ModelType GetRandomModelType()
        {
            const ModelType deformableModelType = ModelType.Cube;

            Array types = Enum.GetValues(typeof(ModelType));
            List<ModelType> allowedTypes = new List<ModelType>();

            foreach (ModelType type in types)
            {
                if (type != deformableModelType)
                {
                    allowedTypes.Add(type);
                }
            }

            return allowedTypes[Random.Range(0, allowedTypes.Count)];
        }

        private void CloseApp()
        {
            System.Diagnostics.Process.GetCurrentProcess().Kill();
        }
    }
}