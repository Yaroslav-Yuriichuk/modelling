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
        [SerializeField] private int _width;
        [SerializeField] private int _height;
        [SerializeField] private int _length;
        [SerializeField] private float _modelSize;

        [Header("Positions")]
        [SerializeField] private Transform _deformableModelPosition;
        [SerializeField] private Transform _targetModelPosition;

        [Header("Controls")]
        [SerializeField] private Button _generateButton;
        [SerializeField] private Button _regenerateButton;

        private const float ShowHideDuration = 1f;
        
        private ModelObject _deformableModel;
        private ModelObject _targetModel;

        private IModellingService _modellingService;
        private IInputService _inputService;
        private IExIntrusionService _exIntrusionService;
        private IModelGeneratingService _modelGeneratingService;
        private IIdentityService _identityService;
        private ILogger _logger;
        
        [Inject]
        private void Construct(IInputService inputService, IModellingService modellingService,
            IExIntrusionService exIntrusionService, IModelGeneratingService modelGeneratingService,
            IIdentityService identityService, ILogger logger,
            ModelObject deformableModel, ModelObject targetModel)
        {
            _modellingService = modellingService;
            _inputService = inputService;
            _exIntrusionService = exIntrusionService;
            _modelGeneratingService = modelGeneratingService;
            _identityService = identityService;
            _logger = logger;
            
            _deformableModel = deformableModel;
            _targetModel = targetModel;
            
            _deformableModel.transform.SetParent(_deformableModelPosition, false);
            
            _targetModel.transform.SetParent(_targetModelPosition, false);
            _targetModel.transform.localScale = Vector3.zero;
        }

        private void OnEnable()
        {
            _generateButton.onClick.AddListener(GenerateTargetModel);
            _regenerateButton.onClick.AddListener(GenerateTargetModel);
        }

        private void OnDisable()
        {
            _generateButton.onClick.RemoveListener(GenerateTargetModel);
            _regenerateButton.onClick.RemoveListener(GenerateTargetModel);
        }

        private void Start()
        {
            _deformableModel.ApplyModel(_modelGeneratingService.GenerateModel(
                new ModelConstraints(ModelType.Cube, _width, _height, _length, _modelSize / _width)));
        }

        private void Update()
        {
            if (_targetModel.Model == null) return;
            
            if (_inputService.GetIntrusionPoint(out Vector3 point))
            {
                _logger.Log($"Intrusion point: {point}");
                _modellingService.AddCommand(new IntrudeCommand(_deformableModel, _exIntrusionService, point));
                _identityService.Calculate(_deformableModel.Model, _targetModel.Model);
                return;
            }

            if (_inputService.GetExtrusionPoint(out point))
            {
                _logger.Log($"Extrusion point: {point}");
                _modellingService.AddCommand(new ExtrudeCommand(_deformableModel, _exIntrusionService, point));
                _identityService.Calculate(_deformableModel.Model, _targetModel.Model);
                return;
            }

            if (_inputService.WantsToUndo())
            {
                _modellingService.UndoLastCommand();
                _identityService.Calculate(_deformableModel.Model, _targetModel.Model);
            }
        }

        private void GenerateTargetModel()
        {
            _generateButton.transform.DOScale(Vector3.zero, ShowHideDuration / 4);

            Sequence sequence = DOTween.Sequence();

            if (_targetModel.Model != null)
            {
                sequence.Append(_targetModel.transform.DOScale(Vector3.zero, ShowHideDuration));
                _regenerateButton.transform.DOScale(Vector3.zero, ShowHideDuration / 3);
            }

            sequence
                .AppendCallback(() =>
                {
                    _targetModel.UpdateCollider = false;
                    _targetModel.ApplyModel(_modelGeneratingService.GenerateModel(
                        new ModelConstraints(GetRandomModelType(), _width, _height, _length, _modelSize / _width)));
                    _regenerateButton.transform.DOScale(Vector3.one, ShowHideDuration / 3);
                })
                .Append(_targetModel.transform.DOScale(Vector3.one, ShowHideDuration))
                .AppendCallback(() =>
                {
                    _identityService.Calculate(_deformableModel.Model, _targetModel.Model);
                });

            
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
    }
}