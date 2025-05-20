<template>
  <div>
    <a-page-header
      :title="isEdit ? '编辑实体' : '创建实体'"
      @back="goBack"
    />

    <a-form
      :model="formState"
      :rules="rules"
      layout="vertical"
      ref="formRef"
    >
      <a-row :gutter="16">
        <a-col :span="12">
          <a-form-item label="实体名称" name="name">
            <a-input v-model:value="formState.name" placeholder="请输入实体名称" />
          </a-form-item>
        </a-col>
        <a-col :span="12">
          <a-form-item label="命名空间" name="namespace">
            <a-input v-model:value="formState.namespace" placeholder="请输入命名空间" />
          </a-form-item>
        </a-col>
      </a-row>

      <a-form-item label="描述" name="description">
        <a-textarea v-model:value="formState.description" placeholder="请输入实体描述" :rows="2" />
      </a-form-item>

      <a-divider>属性列表</a-divider>

      <div class="property-operations">
        <a-button type="primary" @click="addProperty">
          <plus-outlined /> 添加属性
        </a-button>
      </div>

      <a-table
        :columns="propertyColumns"
        :data-source="formState.properties"
        :pagination="false"
        row-key="tempId"
      >
        <template #bodyCell="{ column, record, index }">
          <template v-if="column.key === 'name'">
            <a-input v-model:value="record.name" placeholder="属性名称" />
          </template>
          <template v-else-if="column.key === 'type'">
            <a-select v-model:value="record.type" style="width: 100%">
              <a-select-option value="string">String</a-select-option>
              <a-select-option value="int">Int</a-select-option>
              <a-select-option value="long">Long</a-select-option>
              <a-select-option value="decimal">Decimal</a-select-option>
              <a-select-option value="bool">Bool</a-select-option>
              <a-select-option value="DateTime">DateTime</a-select-option>
              <a-select-option value="Guid">Guid</a-select-option>
              <a-select-option value="enum">Enum</a-select-option>
            </a-select>
          </template>
          <template v-else-if="column.key === 'enumId'">
            <a-select 
              v-if="record.type === 'enum'"
              v-model:value="record.enumId"
              style="width: 100%"
              placeholder="选择枚举"
              :disabled="record.type !== 'enum'"
            >
              <a-select-option v-for="enum_ in enums" :key="enum_.id" :value="enum_.id">
                {{ enum_.name }}
              </a-select-option>
            </a-select>
            <span v-else>-</span>
          </template>
          <template v-else-if="column.key === 'isRequired'">
            <a-checkbox v-model:checked="record.isRequired"></a-checkbox>
          </template>
          <template v-else-if="column.key === 'isKey'">
            <a-checkbox v-model:checked="record.isKey"></a-checkbox>
          </template>
          <template v-else-if="column.key === 'description'">
            <a-input v-model:value="record.description" placeholder="属性描述" />
          </template>
          <template v-else-if="column.key === 'action'">
            <a-button type="link" danger @click="removeProperty(index)">
              <delete-outlined /> 删除
            </a-button>
          </template>
        </template>
      </a-table>

      <div class="form-actions">
        <a-button @click="goBack">取消</a-button>
        <a-button type="primary" @click="submitForm" :loading="submitting">
          保存
        </a-button>
      </div>
    </a-form>
  </div>
</template>

<script setup>
import { ref, reactive, computed, onMounted } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { message } from 'ant-design-vue';
import { PlusOutlined, DeleteOutlined } from '@ant-design/icons-vue';
import api from '../services/api';

const router = useRouter();
const route = useRoute();
const formRef = ref(null);
const submitting = ref(false);
const enums = ref([]);

// 判断是编辑还是创建
const isEdit = computed(() => !!route.params.id);

// 表单状态
const formState = reactive({
  id: 0,
  name: '',
  namespace: 'Sample.Generated',
  description: '',
  properties: []
});

// 表单验证规则
const rules = {
  name: [
    { required: true, message: '请输入实体名称', trigger: 'blur' },
    { pattern: /^[A-Z][a-zA-Z0-9]*$/, message: '实体名称必须以大写字母开头，且只能包含字母和数字', trigger: 'blur' }
  ],
  namespace: [
    { required: true, message: '请输入命名空间', trigger: 'blur' },
    { pattern: /^[a-zA-Z][a-zA-Z0-9.]*$/, message: '命名空间必须以字母开头，且只能包含字母、数字和点', trigger: 'blur' }
  ]
};

// 属性表格列定义
const propertyColumns = [
  {
    title: '名称',
    key: 'name',
    width: '150px'
  },
  {
    title: '类型',
    key: 'type',
    width: '120px'
  },
  {
    title: '枚举',
    key: 'enumId',
    width: '150px'
  },
  {
    title: '必填',
    key: 'isRequired',
    width: '80px',
    align: 'center'
  },
  {
    title: '主键',
    key: 'isKey',
    width: '80px',
    align: 'center'
  },
  {
    title: '描述',
    key: 'description'
  },
  {
    title: '操作',
    key: 'action',
    width: '100px',
    align: 'center'
  }
];

// 添加属性
let tempIdCounter = 0;
const addProperty = () => {
  formState.properties.push({
    tempId: `temp_${tempIdCounter++}`,
    id: 0,
    name: '',
    type: 'string',
    isRequired: true,
    isKey: false,
    description: '',
    enumId: null
  });
};

// 删除属性
const removeProperty = (index) => {
  formState.properties.splice(index, 1);
};

// 获取实体详情
const fetchEntityDetail = async (id) => {
  try {
    const response = await api.entity.getById(id);
    const entity = response.data;
    
    // 更新表单状态
    formState.id = entity.id;
    formState.name = entity.name;
    formState.namespace = entity.namespace || 'Sample.Generated';
    formState.description = entity.description;
    
    // 处理属性，添加临时ID用于表格渲染
    formState.properties = entity.properties.map(prop => ({
      ...prop,
      tempId: `temp_${tempIdCounter++}`
    }));
  } catch (error) {
    message.error('获取实体详情失败');
    console.error(error);
    goBack();
  }
};

// 获取所有枚举
const fetchEnums = async () => {
  try {
    const response = await api.enum.getAll();
    enums.value = response.data;
  } catch (error) {
    message.error('获取枚举列表失败');
    console.error(error);
  }
};

// 提交表单
const submitForm = async () => {
  try {
    await formRef.value.validate();
    
    // 验证属性
    if (formState.properties.length === 0) {
      message.warning('请至少添加一个属性');
      return;
    }
    
    // 验证属性名称
    for (const prop of formState.properties) {
      if (!prop.name) {
        message.warning('属性名称不能为空');
        return;
      }
      if (!/^[A-Z][a-zA-Z0-9]*$/.test(prop.name)) {
        message.warning(`属性名称 ${prop.name} 必须以大写字母开头，且只能包含字母和数字`);
        return;
      }
    }
    
    // 验证主键
    const keyProps = formState.properties.filter(p => p.isKey);
    if (keyProps.length === 0) {
      message.warning('请至少设置一个主键属性');
      return;
    }
    
    submitting.value = true;
    
    // 准备提交的数据，移除临时ID
    const entityData = {
      ...formState,
      properties: formState.properties.map(({ tempId, ...prop }) => prop)
    };
    
    // 创建或更新实体
    if (isEdit.value) {
      await api.entity.update(entityData.id, entityData);
      message.success('更新成功');
    } else {
      await api.entity.create(entityData);
      message.success('创建成功');
    }
    
    goBack();
  } catch (error) {
    console.error(error);
    message.error(isEdit.value ? '更新失败' : '创建失败');
  } finally {
    submitting.value = false;
  }
};

// 返回列表页
const goBack = () => {
  router.push('/entities');
};

onMounted(async () => {
  // 获取所有枚举
  await fetchEnums();
  
  // 如果是编辑模式，获取实体详情
  if (isEdit.value) {
    await fetchEntityDetail(route.params.id);
  } else {
    // 创建模式，默认添加一个ID属性
    addProperty();
    const idProperty = formState.properties[0];
    idProperty.name = 'Id';
    idProperty.type = 'int';
    idProperty.isKey = true;
    idProperty.isRequired = true;
    idProperty.description = '主键ID';
  }
});
</script>

<style scoped>
.property-operations {
  margin-bottom: 16px;
}

.form-actions {
  margin-top: 24px;
  display: flex;
  justify-content: flex-end;
  gap: 8px;
}
</style>